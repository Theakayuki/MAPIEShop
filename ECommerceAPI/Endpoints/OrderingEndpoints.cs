using System.Security.Claims;
using ECommerceAPI.Data;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Models;

namespace ECommerceAPI.Endpoints;

public static class OrderingEndpoints
{
    public static void MapOrderingEndpoints(this WebApplication app)
    {
        app.MapPost("/checkout", CreateOrderFromCartAsync).RequireAuthorization().WithTags("Checkout");
        // map post checkout
        var group = app.MapGroup("/orders").RequireAuthorization().WithTags("Orders");
        group.MapGet("/passed", GetPassedOrdersAsync);
        group.MapGet("/active", GetActiveOrderAsync);
    }
    
    public static async Task<IResult> CreateOrderFromCartAsync(ClaimsPrincipal user, AppDBContext db)
    {
        var userString = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.Identity?.Name;
        if (userString is null) return Results.NotFound();
        var userId = Guid.Parse(userString);
        
        var cart = await db.Carts
            .Include(c => c.Items)
            .ThenInclude(c => c.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);
        
        if (cart is null) return Results.NotFound(new { error = "Active cart not found"});
        if (cart.Items == null || cart.Items.Count <= 0) return Results.BadRequest(new { error = "Cart is empty"});
        
        var order = new Order
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Created,
            Items = [.. cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            })]
        };
        
        order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        
        db.Orders.Add(order);
        cart.Status = CartStatus.CheckedOut;
        await db.SaveChangesAsync();
        
        var result = new
        {
            order.Id,
            order.UserId,
            order.Status,
            order.Total,
            Items = order.Items.Select(i => new
            {
                i.Id,
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice
            })
        };
        
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPassedOrdersAsync(ClaimsPrincipal user, AppDBContext db)
    {
        var userString = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.Identity?.Name;
        if (userString is null) return Results.NotFound();
        var userId = Guid.Parse(userString);

        var orders = await db.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId && (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled))
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
                o.UserId,
                o.Status,
                o.Total,
                o.CreatedAt,
                Items = o.Items.Select(i => new
                {
                    i.Id,
                    i.ProductId,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice
                })
            })
            .ToListAsync();

        return Results.Ok(orders);
    }

    private static async Task<IResult> GetActiveOrderAsync(ClaimsPrincipal user, AppDBContext db)
    {
        var userString = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.Identity?.Name;
        if (userString is null) return Results.NotFound();
        var userId = Guid.Parse(userString);

        var order = await db.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Created)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (order == null) return Results.NotFound(new { error = "No active order found" });

        var result = new
        {
            order.Id,
            order.UserId,
            order.Status,
            order.Total,
            order.CreatedAt,
            Items = order.Items.Select(i => new
            {
                i.Id,
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice
            })
        };

        return Results.Ok(result);
    }
}