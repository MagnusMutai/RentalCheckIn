namespace RentalCheckIn.Services.UI;
public interface IAuthService
{
    Task<OperationResult<LHost>> RegisterAsync(HostSignUpDTO hostSignUpDTO);
    Task<OperationResult<LHost>> LoginAsync(HostLoginDTO hostLoginDTO);
    Task<EmailVerificationResponse> VerifyEmailAsync(string eVerificationToken);
    Task<TokenValidateResult> RefreshTokenAsync();
    Task<OperationResult> ForgotPasswordAsync(ResetRequestDTO resetRequestDTO);
    Task<OperationResult<string>> ResetPasswordAsync(string token, PasswordResetDTO passwordResetDTO);
    Task<OperationResult> VerifyTOTPAsync(TOTPDTO otpModel);
    Task<OperationResult> RegisterFaceIdAsync(uint hostId);
    Task<OperationResult> AuthenticateFaceIdAsync();
}
