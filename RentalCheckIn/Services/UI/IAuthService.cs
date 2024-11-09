using static RentalCheckIn.Components.Pages.PasswordReset;

namespace RentalCheckIn.Services.UI;

public interface IAuthService
{
    Task<AuthenticationResponse> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<AuthenticationResponse> LoginAsync(HostLoginDto hostLoginDto);
    Task<EmailVerificationResponse> VerifyEmailAsync(string eVerificationToken);
    Task<TokenValidateResult> RefreshTokenAsync();
    Task<ResetPasswordResponse> ForgotPasswordAsync(ResetRequestDto resetRequestDto);
    Task<ResetPasswordResponse> ResetPasswordAsync(string token, PasswordResetDto passwordResetDto);
}
