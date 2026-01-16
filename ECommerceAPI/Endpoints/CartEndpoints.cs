using System.Security.Claims;
using ECommerceAPI.Data;

namespace ECommerceAPI.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/cart").RequireAuthorization().WithTags("Cart");
        
    }
}