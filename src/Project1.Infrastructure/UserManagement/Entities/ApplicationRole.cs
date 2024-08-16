using Microsoft.AspNetCore.Identity;

namespace Project1.Infrastructure.UserManagement.Entities;

public class ApplicationRole: IdentityRole<long>
{
    public ICollection<RolePermission> RolePermissions { get; set; }
}
