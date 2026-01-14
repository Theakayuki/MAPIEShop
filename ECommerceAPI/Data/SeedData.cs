using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data;

public static class SeedData
{
    public static async Task SeedProductsAsync(AppDBContext db)
    {
        if (await db.Products.AnyAsync()) return;

        List<Product> products =
        [
            new()
            {
                Name = "Gen T-Shirt",
                Description = "100% cotton unisex tee.",
                Price = 19.99m,
                Stock = 100,
            },
            new()
            {
                Name = "Gen Mug",
                Description = "Ceramic mug for your daily coffee.",
                Price = 9.99m,
                Stock = 200,
            },
            new()
            {
                Name = "Gen Sticker Pack",
                Description = "Set of 5 vinyl stickers.",
                Price = 4.99m,
                Stock = 500,
            }
        ];
        
        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }
}