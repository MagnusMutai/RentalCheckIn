using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Enums;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Components.Pages;

public class PasswordResetBase : ComponentBase
{
    protected bool IsRegistering { get; set; }
    protected PasswordResetDTO ResetPasswordModel { get; set; } = new PasswordResetDTO();
    protected HostLoginDTO AutoHostLoginDTO { get; set; } = new HostLoginDTO();
    protected string? Message { get; set; }
    protected bool IsPasswordResetSuccessful { get; set; }
    protected bool IsLoading { get; set; } = false;
    protected OperationResult<string> ResetPasswordResponse { get; set; }
    [Inject]
    protected IAuthUIService AuthService { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }
    [Inject]
    private ILogger<PasswordResetBase> Logger { get; set; }
    protected async Task HandleResetPassword()
    {
        try
        {
            IsRegistering = true;

            // Parse the current URI to extract query parameters
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("resetToken", out var tokenValues))
            {
                var resetToken = tokenValues.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(resetToken))
                {
                    ResetPasswordResponse = await AuthService.ResetPasswordAsync(resetToken, ResetPasswordModel);

                    if (ResetPasswordResponse != null)
                    {
                        Message = ResetPasswordResponse.Message;

                        if (ResetPasswordResponse.IsSuccess)
                        {
                            AutoHostLoginDTO.Email = ResetPasswordResponse.Data;
                            AutoHostLoginDTO.Password = ResetPasswordModel.NewPassword;

                            var result = await AuthService.LoginAsync(AutoHostLoginDTO);

                            if (result.IsSuccess)
                            {
                                IsPasswordResetSuccessful = true;
                                var lHost = result.Data;

                                // Redirect based on the selected 2FA method
                                if (lHost.AuthenticatorId == (uint)AuthenticatorType.TOTP)
                                {
                                    // Store email for OTP verification
                                    await LocalStorage.SetAsync("emailForTOTP", lHost.MailAddress);
                                    NavigationManager.NavigateTo("/verify-otp");
                                }
                                else if (lHost.AuthenticatorId == (uint)AuthenticatorType.FACEID)
                                {
                                    // Store necessary user ID temporarily for 2FA
                                    await LocalStorage.SetAsync("UserIdFor2FA", lHost.HostId);
                                    NavigationManager.NavigateTo("/verify-faceid");
                                }
                                else
                                {
                                    // Handle cases where no 2FA is selected or unsupported method
                                    Message = Localizer["Unsupported2FAMethod"];
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Message = Localizer["PasswordResetErrorMessage"];
            }
        }
        catch (Exception ex) 
        {
            Message = Localizer["PasswordResetErrorMessage"];
            Logger.LogError(ex, "An unexpected occurred while trying to reset a user's password.");
        }
        finally
        {
            IsRegistering = false;
        }

    }
}
