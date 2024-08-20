using Project1.Core.Users.Interfaces.DTOs;
using System.Security.Claims;
using System.Security;
using System.Text;

namespace Project1.Core.Users.Interfaces;

public interface IAuthenticationService
{
    Task<string> AuthenticateAsync(LoginRequestDTO loginRequest);
    Task<RegisterResultDTO> RegisterAsync(RegisterRequestDTO registerRequest);

    Task<List<PermissionDTO>> GetUserPermissionsAsync(long userId);

    Task<List<PermissionDTO>> GetRolePermissionsAsync(long userId);

    Task<bool> AddPermissionToUserAsync(UserPermissionDTO input);

    Task<bool> RemovePermissionFromUserAsync(UserPermissionDTO input);

    Task<bool> AddPermissionToRoleAsync(RolePermissionDTO input);

    Task<bool> RemovePermissionFromRoleAsync(RolePermissionDTO input);

    Task<bool> AssignRoleToUserAsync(UserRoleDTO input);

    Task<bool> RemoveRoleFromUserAsync(UserRoleDTO input);

    Task<List<string>> GetUserRolesAsync(long userId);

    Task<bool> CheckUserPermissionAsync(UserPermissionDTO input);

    Task<bool> CheckRolePermissionAsync(RolePermissionDTO input);

    Task<RoleDTO> AddRoleAsync(string roleName);

    Task<PermissionDTO> AddPermissionAsync(string permissionName);
}
