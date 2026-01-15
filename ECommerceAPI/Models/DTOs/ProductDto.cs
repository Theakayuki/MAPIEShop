using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models.DTOs;

public record ProductDto(
    Guid Id,
    [Required, StringLength(200)]
    string Name,
    string? Description,
    [Range(0.0, double.MaxValue)]
    decimal Price,
    [Range(0, int.MaxValue)]
    int Stock,
    [Url]
    string? ImageUrl
);