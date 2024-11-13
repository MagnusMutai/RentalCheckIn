namespace RentalCheckIn.Services.UI;
public interface IAuthService
{
    Task<OperationResult<LHost>> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<OperationResult<LHost>> LoginAsync(HostLoginDto hostLoginDto);
    Task<EmailVerificationResponse> VerifyEmailAsync(string eVerificationToken);
    Task<TokenValidateResult> RefreshTokenAsync();
    Task<OperationResult> ForgotPasswordAsync(ResetRequestDto resetRequestDto);
    Task<OperationResult<string>> ResetPasswordAsync(string token, PasswordResetDto passwordResetDto);
}
