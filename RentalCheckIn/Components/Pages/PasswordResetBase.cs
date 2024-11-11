using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class PasswordResetBase : ComponentBase
{
    protected bool ShouldSpin;

    protected PasswordResetDto resetPasswordModel = new PasswordResetDto();
    protected HostLoginDto autoHostLoginDto = new HostLoginDto();
    protected string Message { get; set; }
    protected bool IsPasswordResetSuccessful { get; set; } = false;
    protected bool IsLoading { get; set; } = false;
    protected ResetPasswordResponse ResetPasswordResponse { get; set; }

    [Inject]
    protected IAuthService AuthService { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected ProtectedLocalStorage LocalStorage { get; set; }
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
                        if (ResetPasswordResponse.Success)
                        {
                            autoHostLoginDto.Email = ResetPasswordResponse.Email;
                            autoHostLoginDto.Password = resetPasswordModel.NewPassword;

                            var result = await AuthService.LoginAsync(autoHostLoginDto);
                            if (result.Success)
                            {
                                var lHost = result.Host;
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
                Message = "Invalid request.";
            }
        }
        catch (Exception ex) 
        {
            Message = "Invalid request.";
        }

        ShouldSpin = false;
    }
}
