using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
namespace RentalCheckIn.Components.Pages;

public class ConfirmEmailBase : ComponentBase
{
    protected bool isLoading = true;
    protected bool isSuccess = false;
    protected string? Message;
    protected EmailVerificationResponse verificationResult;
    public string? DisplayToast { get; set; } = "d-block";
    public string BackGroundColor { get; set; } = "bg-success";
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IAuthService AuthService { get; set; }
    [Inject]
    private ILogger<ConfirmEmailBase> Logger { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }


    protected override async Task OnInitializedAsync()
    {
        try
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
                    BackGroundColor = "bg-success";
                    Message = verificationResult.Message;
                    DisplayToast = "d-block";
                }
                else
                {
                    BackGroundColor = "bg-danger";
                    Message = verificationResult.Message;
                    DisplayToast = "d-block";
                }
            }
            else
            {
                // No verification token was provided in the URL.
                BackGroundColor = "bg-danger";
                Message = Localizer["VerificationInvalid"];
                DisplayToast = "d-block";
            }
            DisplayToast = DisplayToast ?? "d-block";

        }
        catch(Exception ex)
        {
            BackGroundColor = "bg-danger";
            Message = Localizer["UnexpectedErrorOccurred"];
            DisplayToast = "d-block";
            Logger.LogError(ex, "An unexpected occured during email verification in Email confirmation page");
        }
        finally
        {
            isLoading = false;
        }
    }

    protected void HandleLogin()
    {
        NavigationManager.NavigateTo("login");
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
