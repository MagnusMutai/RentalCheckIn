using static RentalCheckIn.DTOs.CustomResponses;

namespace RentalCheckIn.Services.UI;

public interface IAuthService
{
    Task<AuthenticationResponse> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<AuthenticationResponse> LoginAsync(HostLoginDto hostLoginDto);
}
