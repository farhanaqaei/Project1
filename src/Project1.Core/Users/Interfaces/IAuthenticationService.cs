using Project1.Core.Users.Interfaces.DTOs;

namespace Project1.Core.Users.Interfaces;

public interface IAuthenticationService
{
    Task<string> AuthenticateAsync(LoginRequestDTO loginRequest);
    Task<RegisterResultDTO> RegisterAsync(RegisterRequestDTO registerRequest);
}
