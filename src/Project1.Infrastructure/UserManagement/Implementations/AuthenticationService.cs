using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project1.Core.Users.Interfaces;
using Project1.Core.Users.Interfaces.DTOs;
using Project1.Infrastructure.UserManagement.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project1.Infrastructure.UserManagement.Implementations;

public class AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IAuthenticationService
{
    public async Task<string> AuthenticateAsync(LoginRequestDTO loginRequest)
    {
        var user = await userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
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
            return RegisterResultDTO.Success();
        }

        var errors = identityResult.Errors.Select(e => e.Description);
        return RegisterResultDTO.Failure(errors);
    }
}
