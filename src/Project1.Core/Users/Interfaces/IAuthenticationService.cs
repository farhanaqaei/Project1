using Project1.Core.Users.Interfaces.DTOs;
using System.Security.Claims;
using System.Security;
using System.Text;

namespace Project1.Core.Users.Interfaces;

public interface IAuthenticationService
{
    #region authentication methods

    Task<string> AuthenticateAsync(LoginRequestDTO loginRequest);
    Task<RegisterResultDTO> RegisterAsync(RegisterRequestDTO registerRequest);

    #endregion

    #region user role management

    Task<bool> AssignRoleToUserAsync(UserRoleDTO input);
    Task<bool> RemoveRoleFromUserAsync(UserRoleDTO input);
    Task<List<string>> GetUserRolesAsync(long userId);

    #endregion

    #region user permission management

    Task<List<PermissionDTO>> GetUserPermissionsAsync(long userId);
    Task<bool> AddPermissionToUserAsync(UserPermissionDTO input);
    Task<bool> RemovePermissionFromUserAsync(UserPermissionDTO input);
    Task<bool> CheckUserPermissionAsync(UserPermissionDTO input);

    #endregion

    #region role permission management

    Task<List<PermissionDTO>> GetRolePermissionsAsync(long userId);
    Task<bool> AddPermissionToRoleAsync(RolePermissionDTO input);
    Task<bool> RemovePermissionFromRoleAsync(RolePermissionDTO input);
    Task<bool> CheckRolePermissionAsync(RolePermissionDTO input);

    #endregion

    #region role management

    Task<RoleDTO> AddRoleAsync(string roleName);
    Task<bool> RemoveRoleAsync(string roleName);
    Task<List<RoleDTO>> GetAllRolesAsync();

    #endregion

    #region permission management

    Task<PermissionDTO> AddPermissionAsync(string permissionName);
    Task<bool> RemovePermissionAsync(string permissionName);
    Task<List<PermissionDTO>> GetAllPermissionsAsync();
    Task<List<PermissionDTO>> GetAllPermissionsAsync(string roleName);

    #endregion
}
