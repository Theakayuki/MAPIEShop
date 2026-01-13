
using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Models;

public class User : IdentityUser<Guid>
{
}

public class AppRole : IdentityRole<Guid>
{
}