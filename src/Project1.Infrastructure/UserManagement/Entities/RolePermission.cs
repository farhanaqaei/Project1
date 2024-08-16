using Microsoft.AspNetCore.Identity;

namespace Project1.Infrastructure.UserManagement.Entities;

public class RolePermission
{
    public long RoleId { get; set; }
    public ApplicationRole Role { get; set; }

    public long PermissionId { get; set; }
    public Permission Permission { get; set; }
}
