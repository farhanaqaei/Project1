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

    private async Task<List<Permission>> GetUserPermissionsAsync(long userId)
    {
        return await db.UserPermissions
            .Where(up => up.UserId == userId && up.IsGranted)
            .Select(up => up.Permission)
            .ToListAsync();
    }

    private async Task<List<Permission>> GetRolePermissionsAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var permissions = new List<Permission>();

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var rolePermissions = await db.RolePermissions
                    .Where(rp => rp.RoleId == role.Id)
                    .Select(rp => rp.Permission)
                    .ToListAsync();

                permissions.AddRange(rolePermissions);
            }
        }

        return permissions.Distinct().ToList();
    }
}
