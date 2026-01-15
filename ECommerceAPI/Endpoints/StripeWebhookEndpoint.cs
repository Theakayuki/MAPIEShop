using ECommerceAPI.Data;
using ECommerceAPI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace ECommerceAPI.Endpoints;

public static class StripeWebhookEndpoint
{
    public static void MapStripeWebhook(this WebApplication app)
    {
        var group = app.MapGroup("/webhooks/stripe").WithTags(["Webhooks", "Stripe"]);
        group.MapPost("/", StripeWebhookAsync);
    }
    
    private static async Task<IResult> StripeWebhookAsync(HttpRequest req, PaymentService payments, AppDBContext db)
    {
        var json = await new StreamReader(req.Body).ReadToEndAsync();
        var sig = req.Headers["Stripe-Signature"].FirstOrDefault();
        var ev = payments.ConstructEvent(json, sig ?? string.Empty);
        if (ev == null) return Results.BadRequest();

        if (ev.Type == "payment_intent.succeeded")
        {
            if (ev.Data.Object is PaymentIntent intent)
            {
                var payment = await db.Payments.FirstOrDefaultAsync(p => p.StripePaymentIntentId == intent.Id);
                if (payment != null)
                {
                    payment.Status = intent.Status;
                    await db.SaveChangesAsync();
                }
            }
        }
        
        return Results.Ok();
    }
}