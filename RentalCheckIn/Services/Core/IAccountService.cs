using static RentalCheckIn.DTOs.CustomResponses;

namespace RentalCheckIn.Services.Core;

public interface IAccountService
{
    Task<AuthenticationResult> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<AuthenticationResult> LoginAsync(HostLoginDto hostLoginDto);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
    Task<EmailVerificationResult> VerifyEmailTokenAsync(string token);
    Task<RefreshTokenResult> GetRefreshTokenAsync(string refreshToken);
    Task<LHost> GetLHostByIdAsync(uint id);
}
