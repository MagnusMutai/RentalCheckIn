using static RentalCheckIn.DTOs.CustomResponses;

namespace RentalCheckIn.Services.Core;

public interface IAccountService
{
    Task<AuthenticationResponse> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<AuthenticationResponse> LoginAsync(HostLoginDto hostLoginDto);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
    Task<EmailVerificationResponse> VerifyEmailTokenAsync(string token);
    Task<RefreshTokenResponse> GetRefreshTokenAsync(string refreshToken);
    Task<LHost> GetLHostByIdAsync(uint id);
}
