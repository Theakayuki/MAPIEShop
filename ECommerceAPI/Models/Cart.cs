using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models;

public class Cart
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public CartStatus Status { get; set; } = CartStatus.Active;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public ICollection<CartItem> Items { get; set; } = [];
}

public enum CartStatus
{
    Active,
    CheckedOut
}