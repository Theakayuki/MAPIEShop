using System.Security.Authentication;
using System.Security.Claims;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/cart").RequireAuthorization().WithTags("Cart");

        group.MapGet("/", GetCartAsync);
        group.MapPost("/items", AddCartItemAsync);
        group.MapDelete("/items/{id:guid}", RemoveCartItemAsync);
    }

    private static async Task<IResult> GetCartAsync(ClaimsPrincipal user, AppDBContext db)
    {
        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.Identity?.Name;
        if (userIdStr == null) return Results.Unauthorized();
        var userId = Guid.Parse(userIdStr);

        var cart = await db.Carts
            .Include(c => c.Items)
            .ThenInclude(c => c.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);
        if (cart == null)
        {
            var newCart = new Cart { UserId = userId };
            db.Carts.Add(newCart);
            await db.SaveChangesAsync();
            return Results.Ok(new CartDto(newCart.Id, []));
        }

        return Results.Ok(MapCartDto(cart));
    }

    private static async Task<IResult> AddCartItemAsync([FromBody] AddCartItemRequest req, ClaimsPrincipal user, AppDBContext db)
    {
        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.Identity?.Name;
        if (userIdStr == null) return Results.Unauthorized();
        var userId = Guid.Parse(userIdStr);

        var cart = await db.Carts
            .Include(c => c.Items)
            .ThenInclude(c => c.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            db.Carts.Add(cart);
        }

        var product = await db.Products.FindAsync(req.ProductId);
        if (product == null) return Results.NotFound();

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity = req.Quantity;
            existing.UnitPrice = product.Price;
            existing.Product = product;
            await db.SaveChangesAsync();
            return Results.Ok(MapCartDto(cart));
        }
        
        var item = new CartItem { Cart = cart, Product = product, ProductId = product.Id, Quantity = req.Quantity, UnitPrice = product.Price };
        cart.Items.Add(item);
        await db.SaveChangesAsync();
        return Results.Ok(MapCartDto(cart));
    }
    
    private static async Task<IResult> RemoveCartItemAsync(Guid id, ClaimsPrincipal user, AppDBContext db)
    {
        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.Identity?.Name;
        if (userIdStr == null) return Results.Unauthorized();
        var userId = Guid.Parse(userIdStr);
        
        var item = await db.CartItems.Include(ci => ci.Cart).FirstOrDefaultAsync(ci => ci.Id == id);
        if (item == null) return Results.NotFound();
        if (item.Cart == null || item.Cart.UserId != userId) return Results.NotFound();

        db.CartItems.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(MapCartDto(item.Cart));
    }

    private static CartDto MapCartDto(Cart cart)
    {
        return new CartDto(
            cart.Id,
             [.. cart.Items.Select(i => new CartItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name ?? string.Empty,
                i.Quantity,
                i.UnitPrice,
                i.Quantity * i.UnitPrice
            ))]
        );
    }
}