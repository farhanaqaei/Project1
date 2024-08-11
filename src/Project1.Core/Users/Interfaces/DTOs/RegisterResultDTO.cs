namespace Project1.Core.Users.Interfaces.DTOs;

public class RegisterResultDTO
{
    public bool Succeeded { get; set; }
    public IEnumerable<string> Errors { get; set; }

    public static RegisterResultDTO Success()
    {
        return new RegisterResultDTO { Succeeded = true, Errors = Enumerable.Empty<string>() };
    }

    public static RegisterResultDTO Failure(IEnumerable<string> errors)
    {
        return new RegisterResultDTO { Succeeded = false, Errors = errors };
    }
}
