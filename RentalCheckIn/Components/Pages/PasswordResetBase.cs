using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Components.Pages;

public class PasswordResetBase : ComponentBase
{
    protected bool ShouldSpin;

    protected PasswordResetDTO resetPasswordModel = new PasswordResetDTO();
    protected HostLoginDTO autoHostLoginDTO = new HostLoginDTO();
    protected string Message { get; set; }
    protected bool IsPasswordResetSuccessful { get; set; }
    protected bool IsLoading { get; set; } = false;
    protected OperationResult<string> ResetPasswordResponse { get; set; }

    [Inject]
    protected IAuthService AuthService { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }
    protected async Task HandleResetPassword()
    {

        ShouldSpin = true;
        try
        {

            // Parse the current URI to extract query parameters
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("resetToken", out var tokenValues))
            {
                var eVerificationToken = tokenValues.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(eVerificationToken))
                {
                    ResetPasswordResponse = await AuthService.ResetPasswordAsync(eVerificationToken, resetPasswordModel);
                    if (ResetPasswordResponse != null)
                    {
                        Message = ResetPasswordResponse.Message;
                        if (ResetPasswordResponse.IsSuccess)
                        {
                            autoHostLoginDTO.Email = ResetPasswordResponse.Data;
                            autoHostLoginDTO.Password = resetPasswordModel.NewPassword;

                            var result = await AuthService.LoginAsync(autoHostLoginDTO);
                            if (result.IsSuccess)
                            {
                                IsPasswordResetSuccessful = true;
                                var lHost = result.Data;
                                // Store email for OTP verification
                                await LocalStorage.SetAsync("emailForOtp", lHost.MailAddress);
                                NavigationManager.NavigateTo("/verify-otp");
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
        }

        ShouldSpin = false;
    }
}
