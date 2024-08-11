using Microsoft.AspNetCore.Mvc;
using Project1.Core.Users.Interfaces;
using Project1.Core.Users.Interfaces.DTOs;

namespace Project1.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
    {
        var result = await authenticationService.RegisterAsync(registerRequest);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { Message = "User registered successfully!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        var token = await authenticationService.AuthenticateAsync(loginRequest);
        return Ok(new { Token = token });
    }
}
