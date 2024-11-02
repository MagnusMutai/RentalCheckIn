using static RentalCheckIn.Responses.CustomResponses;

namespace RentalCheckIn.Services;

public interface IAuthService
{
    Task<AuthenticationResult> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<AuthenticationResult> LoginAsync(HostLoginDto hostLoginDto);
}
