namespace Project1.Infrastructure.UserManagement.Entities;

public class UserPermission
{
    public long UserId { get; set; }
    public ApplicationUser User { get; set; }

    public long PermissionId { get; set; }
    public Permission Permission { get; set; }

    public bool IsGranted { get; set; }
}
