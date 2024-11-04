using static RentalCheckIn.DTOs.CustomResponses;

namespace RentalCheckIn.Services.UI;

public interface IAuthService
{
    Task<AuthenticationResult> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<AuthenticationResult> LoginAsync(HostLoginDto hostLoginDto);
}
