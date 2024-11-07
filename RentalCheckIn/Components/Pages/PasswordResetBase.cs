using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
namespace RentalCheckIn.Components.Pages;

public class PasswordResetBase : ComponentBase
{

    protected PasswordResetDto resetPasswordModel = new PasswordResetDto();
    protected string ErrorMessage { get; set; }
    protected bool IsPasswordResetSuccessful { get; set; } = false;
    protected bool IsLoading { get; set; } = false;
    protected ResetPasswordResponse ResetPasswordResponse { get; set; }

    [Inject]
    protected IAuthService AuthService { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    protected async Task HandleResetPassword()
    {

        IsLoading = true;
        // Parse the current URI to extract query parameters
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue("resetToken", out var tokenValues))
        {
            var eVerificationToken = tokenValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(eVerificationToken))
            {
                ResetPasswordResponse = await AuthService.ResetPasswordAsync(eVerificationToken, resetPasswordModel);
                if (!ResetPasswordResponse.Success)
                {
                    ErrorMessage = ResetPasswordResponse.Message;
                }
            }
        }

        IsLoading = false;

    }
}
