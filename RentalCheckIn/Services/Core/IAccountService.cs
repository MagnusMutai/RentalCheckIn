namespace RentalCheckIn.Services.Core;

public interface IAccountService
{
    Task<OperationResult<LHost>> RegisterAsync(HostSignUpDTO hostSignUpDTO);
    Task<OperationResult<HostLoginResponseDTO>> LoginAsync(HostLoginDTO hostLoginDTO);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
    Task<EmailVerificationResponse> VerifyEmailTokenAsync(string token);
    Task<OperationResult<RefreshToken>> GetRefreshTokenAsync(string refreshToken);
    Task<OperationResult<LHost>> GetLHostByIdAsync(uint id);
    Task <LHost>GetLHostByEmailAsync(string email);
    Task<OperationResult> ForgotPasswordAsync(LHost lHost);
    Task<OperationResult<string>> ResetPasswordAsync(PasswordResetRequest passwordRequest);
}
