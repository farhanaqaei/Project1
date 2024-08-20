using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project1.Core.Generals.Interfaces;
using Project1.Core.Users.Interfaces;
using Project1.Core.Users.Interfaces.DTOs;
using Project1.Infrastructure.Cache;
using Project1.Infrastructure.Data;
using Project1.Infrastructure.UserManagement.Entities;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project1.Infrastructure.UserManagement.Implementations;

public class AuthenticationService(ApplicationDbContext db, ICacheManager cacheManager, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration) : IAuthenticationService
{
    public async Task<string> AuthenticateAsync(LoginRequestDTO loginRequest)
    {
        var user = await userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var userPermissions = await GetUserPermissionsAsync(user.Id);

        // still need to think work it out
        //var cacheKeyRoles = $"Roles_User_{user.Id}";
        //var cacheKeyPermissions = $"Permissions_User_{user.Id}";
        //cacheManager.Set(cacheKeyRoles, userRoles, TimeSpan.FromHours(1));
        //cacheManager.Set(cacheKeyPermissions, userPermissions, TimeSpan.FromHours(1));

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in userPermissions)
        {
            claims.Add(new Claim(type: "Permission", permission.Name));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(configuration["JwtSettings:ExpiryMinutes"])),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<RegisterResultDTO> RegisterAsync(RegisterRequestDTO registerRequest)
    {
        //if (registerRequest.Password != registerRequest.ConfirmPassword)
        //{
        //    return RegisterResultDTO.Failure(new[] { "Passwords do not match." });
        //}

        var user = new ApplicationUser
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email
        };

        var identityResult = await userManager.CreateAsync(user, registerRequest.Password);

        if (identityResult.Succeeded)
        {
            var roleResult = await userManager.AddToRoleAsync(user, "User");

            if (roleResult.Succeeded)
            {
                return RegisterResultDTO.Success();
            }
            else
            {
                await userManager.DeleteAsync(user);
                var errors = roleResult.Errors.Select(e => e.Description);
                return RegisterResultDTO.Failure(errors);
            }
        }

        var creationErrors = identityResult.Errors.Select(e => e.Description);
        return RegisterResultDTO.Failure(creationErrors);
    }

    public async Task<List<PermissionDTO>> GetUserPermissionsAsync(long userId)
    {
        return await db.UserPermissions
            .Where(up => up.UserId == userId && up.IsGranted)
            .Select(up => new PermissionDTO
            {
                Id = up.Permission.Id,
                Name = up.Permission.Name,
                Description = up.Permission.Description,
            })
            .ToListAsync();
    }

    public async Task<List<PermissionDTO>> GetRolePermissionsAsync(long userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        var roles = await userManager.GetRolesAsync(user);
        var permissions = new List<PermissionDTO>();

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var rolePermissions = await db.RolePermissions
                    .Where(rp => rp.RoleId == role.Id)
                    .Select(rp => new PermissionDTO
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name,
                        Description = rp.Permission.Description
                    })
                    .ToListAsync();

                permissions.AddRange(rolePermissions);
            }
        }

        return permissions.Distinct().ToList();
    }

    public async Task<bool> AddPermissionToUserAsync(UserPermissionDTO input)
    {
        var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Name == input.PermissionName);
        if (permission == null) return false;

        var user = await userManager.FindByIdAsync(input.UserId.ToString());
        var roles = await userManager.GetRolesAsync(user);

        // Check if the permission is already granted by any of the user's roles
        bool isGrantedByRole = false;
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var rolePermission = await db.RolePermissions
                    .AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id);

                if (rolePermission)
                {
                    isGrantedByRole = true;
                    break;
                }
            }
        }

        var userPermission = await db.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == user.Id && up.PermissionId == permission.Id);

        if (isGrantedByRole)
        {
            // If the permission is granted via a role, and a userPermission record exists, remove it
            if (userPermission != null)
            {
                db.UserPermissions.Remove(userPermission);
                await db.SaveChangesAsync();
            }
        }
        else
        {
            // If the permission is not granted via roles, ensure it's granted directly
            if (userPermission == null)
            {
                userPermission = new UserPermission
                {
                    UserId = user.Id,
                    PermissionId = permission.Id,
                    IsGranted = true
                };
                db.UserPermissions.Add(userPermission);
            }
            else if (!userPermission.IsGranted)
            {
                userPermission.IsGranted = true;
                db.UserPermissions.Update(userPermission);
            }

            await db.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> RemovePermissionFromUserAsync(UserPermissionDTO input)
    {
        var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Name == input.PermissionName);
        if (permission == null) return false;

        var user = await userManager.FindByIdAsync(input.UserId.ToString());
        var roles = await userManager.GetRolesAsync(user);

        var roleHasPermission = await db.RolePermissions
            .AnyAsync(rp => roles.Contains(rp.Role.Name) && rp.PermissionId == permission.Id);

        var userPermission = await db.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == input.UserId && up.PermissionId == permission.Id);

        if (roleHasPermission)
        {
            // If the role grants this permission and the userPermission doesn't exist or is currently granted,
            // create or update the userPermission to explicitly deny it
            if (userPermission == null)
            {
                userPermission = new UserPermission
                {
                    UserId = input.UserId,
                    PermissionId = permission.Id,
                    IsGranted = false
                };
                db.UserPermissions.Add(userPermission);
            }
            else if (userPermission.IsGranted)
            {
                userPermission.IsGranted = false;
                db.UserPermissions.Update(userPermission);
            }
        }
        else
        {
            // If no role grants this permission, remove the UserPermission if it exists
            if (userPermission != null)
            {
                db.UserPermissions.Remove(userPermission);
            }
        }

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddPermissionToRoleAsync(RolePermissionDTO input)
    {
        var role = await roleManager.FindByNameAsync(input.RoleName);
        if (role == null) return false;

        var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Name == input.PermissionName);
        if (permission == null) return false;

        var rolePermission = await db.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id);

        if (rolePermission == null)
        {
            rolePermission = new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            };
            db.RolePermissions.Add(rolePermission);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> RemovePermissionFromRoleAsync(RolePermissionDTO input)
    {
        var role = await roleManager.FindByNameAsync(input.RoleName);
        if (role == null) return false;

        var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Name == input.PermissionName);
        if (permission == null) return false;

        var rolePermission = await db.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id);

        if (rolePermission != null)
        {
            db.RolePermissions.Remove(rolePermission);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> AssignRoleToUserAsync(UserRoleDTO input)
    {
        var user = await userManager.FindByIdAsync(input.UserId.ToString());
        if (user == null) return false;

        var result = await userManager.AddToRoleAsync(user, input.RoleName);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRoleFromUserAsync(UserRoleDTO input)
    {
        var user = await userManager.FindByIdAsync(input.UserId.ToString());
        if (user == null) return false;

        var result = await userManager.RemoveFromRoleAsync(user, input.RoleName);
        return result.Succeeded;
    }

    public async Task<List<string>> GetUserRolesAsync(long userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new List<string>();

        return new List<string>(await userManager.GetRolesAsync(user));
    }

    public async Task<bool> CheckUserPermissionAsync(UserPermissionDTO input)
    {
        var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Name == input.PermissionName);
        if (permission == null) return false;

        var userPermission = await db.UserPermissions
            .AnyAsync(up => up.UserId == input.UserId && up.PermissionId == permission.Id && up.IsGranted);

        if (userPermission) return true;

        var user = await userManager.FindByIdAsync(input.UserId.ToString());
        if (user == null) return false;

        var rolePermissions = await GetRolePermissionsAsync(user.Id);
        return rolePermissions.Any(p => p.Name == input.PermissionName);
    }

    public async Task<bool> CheckRolePermissionAsync(RolePermissionDTO input)
    {
        var role = await roleManager.FindByNameAsync(input.RoleName);
        if (role == null) return false;

        var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Name == input.PermissionName);
        if (permission == null) return false;

        return await db.RolePermissions
            .AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id);
    }

    public async Task<PermissionDTO> AddPermissionAsync(string permissionName)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
        {
            throw new ArgumentException("Permission name cannot be empty.", nameof(permissionName));
        }

        var existingPermission = await db.Permissions
                                         .FirstOrDefaultAsync(p => p.Name == permissionName);
        if (existingPermission != null)
        {
            throw new InvalidOperationException("Permission already exists.");
        }

        var permission = new Permission { Name = permissionName };
        db.Permissions.Add(permission);
        await db.SaveChangesAsync();

        return new PermissionDTO { Id = permission.Id, Name = permission.Name };
    }

    public async Task<RoleDTO> AddRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentException("Role name cannot be empty.", nameof(roleName));
        }

        var existingRole = await roleManager.FindByNameAsync(roleName);
        if (existingRole != null)
        {
            throw new InvalidOperationException("Role already exists.");
        }

        var role = new ApplicationRole { Name = roleName };
        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return new RoleDTO { Id = role.Id, Name = role.Name };
    }
}
