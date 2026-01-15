using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products");

        group.MapGet("/", GetAllProductsAsync);
        group.MapGet("/{id:guid}", GetProductAsync);
        group.MapPost("/", CreateProductAsync).RequireAuthorization("AdminOnly");
        group.MapPut("/{id:guid}", UpdateProductAsync).RequireAuthorization("AdminOnly");
        group.MapDelete("/{id:guid}", DeleteProductAsync).RequireAuthorization("AdminOnly");
    }

    private static async Task<IResult> GetAllProductsAsync(AppDBContext db)
    {
        var list = await db.Products
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock,
                p.ImageUrl
            ))
            .ToListAsync();
        return Results.Ok(list);
    }

    public static async Task<IResult> GetProductAsync(Guid id, AppDBContext db)
    {
        var product = await db.Products.FindAsync(id);
        return product is null ? Results.NotFound() : Results.Ok(new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock,
                product.ImageUrl
        ));
    }

    private static async Task<IResult> CreateProductAsync([FromBody] Product product, AppDBContext db)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var dto = new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock,
                product.ImageUrl
        );
        return Results.Created($"/products/{product.Id}", dto);
    }

    private static async Task<IResult> UpdateProductAsync(Guid id, [FromBody] Product update, AppDBContext db)
    {
        var p = await db.Products.FindAsync(id);
        if (p == null) return Results.NotFound();

        p.Name = update.Name;
        p.Description = update.Description;
        p.Price = update.Price;
        p.Stock = update.Stock;
        await db.SaveChangesAsync();

        var dto = new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Stock,
            p.ImageUrl
        );
        return Results.Ok(dto);
    }

    private static async Task<IResult> DeleteProductAsync(Guid id, AppDBContext db)
    {
        var p = await db.Products.FindAsync(id);
        if (p == null) return Results.NotFound();

        db.Products.Remove(p);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}