namespace ECommerceAPI.Models.DTOs;

public record CartDto(
    Guid Id,
    List<CartItemDto> Items
);