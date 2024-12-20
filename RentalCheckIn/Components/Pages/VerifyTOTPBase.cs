using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
namespace RentalCheckIn.Components.Pages;

public class VerifyTOTPBase : ComponentBase
{
    protected TOTPDTO OTPModel { get; set; } = new();
    protected string? ErrorMessage { get; set; }
    protected bool IsRegistering { get; set; }
    public string? DisplayToast { get; set; } = "d-block";

    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private IAuthUIService AuthService { get; set; }
    [Inject]
    private ILogger<VerifyTOTPBase> Logger { get; set; }

    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            var result = await LocalStorage.GetAsync<string>("emailForTOTP");

            if (result.Success)
            {
                OTPModel.Email = result.Value;
            }
            if (string.IsNullOrEmpty(OTPModel.Email))
            {
                NavigationManager.NavigateTo("/login");
            }
        }
        catch (Exception ex) 
        {
            Logger.LogError(ex, "An unexpected error occurred while trying to check for emailForTOTP in LocalStorage.");
        }
    }

    protected async Task HandleVerifyOtp()
    {
        IsRegistering = true;
        try
        {
            var result = await AuthService.VerifyTOTPAsync(OTPModel);
           
            if (result.IsSuccess)
            {
                await LocalStorage.DeleteAsync("emailForTOTP");
            }
            else
            {
                ErrorMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = Localizer["UnexpectedErrorOccurred"];
            Logger.LogError(ex, "An unexpected error occurred while trying to verify TOTP in VerifyTOTP component");
        }
        finally
        {
            IsRegistering = false;
        }

    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }


}
