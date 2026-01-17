using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models.DTOs;

public record AddCartItemRequest(
    [Required]
    Guid ProductId,
    [Range(1, int.MaxValue)]
    int Quantity
);