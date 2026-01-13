using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Range(0.0, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    [Url]
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}