using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models;

public class CartItem
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}