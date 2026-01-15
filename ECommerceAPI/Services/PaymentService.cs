using ECommerceAPI.Models;
using Stripe;

namespace ECommerceAPI.Services;

public class PaymentService
{
    private readonly StripeSettings _settings;

    public PaymentService(StripeSettings settings)
    {
        _settings = settings;
        if (!string.IsNullOrEmpty(_settings.SecretKey))
        {
             StripeConfiguration.ApiKey = _settings.SecretKey;
        }
    }
    
    public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency = "usd")
    {
        var service = new PaymentIntentService();
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = currency,
            PaymentMethodTypes = ["card"]
        };
        return await service.CreateAsync(options);
    }
    
    public Event? ConstructEvent(string json, string stripeSignatureHeader)
    {
        if (string.IsNullOrEmpty(_settings.WebhookSecret))
        {
            return null;
        }
        
        try
        {
            return EventUtility.ConstructEvent(json, stripeSignatureHeader, _settings.WebhookSecret);
        }
        catch
        {
            return null;
        }
    }
}