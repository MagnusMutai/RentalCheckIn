using static RentalCheckIn.Responses.CustomResponses;

namespace RentalCheckIn.BusinessServices;

public interface IAccountService
{
    Task<AuthenticationResult> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<AuthenticationResult> LoginAsync(HostLoginDto hostLoginDto);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
}
