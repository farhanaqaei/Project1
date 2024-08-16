using Microsoft.AspNetCore.Identity;

namespace Project1.Infrastructure.UserManagement.Entities;

public class ApplicationUser : IdentityUser<long>
{
    public ICollection<UserPermission> UserPermissions { get; set; }
}
