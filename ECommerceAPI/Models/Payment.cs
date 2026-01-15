using System.ComponentModel.DataAnnotations;

public class Payment
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string? UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set;} = "usd";
    public string? Status { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}