using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    
    public List<OrderItem> Items { get; set ;} = [];
}

public enum OrderStatus
{
    Created,
    Paid,
    Completed,
    Cancelled
}