using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class ConfirmEmailBase : ComponentBase
{
    protected bool isLoading = true;
    protected bool isSuccess = false;
    protected string? errorMessage;
    protected string? Message;
    protected EmailVerificationResponse verificationResult;
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IAuthService AuthService { get; set; }

    protected async Task HandleVerifyEmail()
    {
        // Parse the current URI to extract query parameters
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue("emailToken", out var tokenValues))
        {
            var eVerificationToken = tokenValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(eVerificationToken))
            {
                verificationResult = await AuthService.VerifyEmailAsync(eVerificationToken);
            }

            if (verificationResult.IsSuccess)
            {
                isSuccess = true;
                Message = verificationResult.Message;
            }
            else
            {
                errorMessage = verificationResult.Message;
            }
        }
        else
        {
            // No verification token was provided in the URL.
            errorMessage = "Verification is invalid, please contact support";
        }

        isLoading = false;
    }
}
