using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
namespace RentalCheckIn.Components.Pages;

public class VerifyTOTPBase : ComponentBase
{
    protected TOTPDTO oTPModel = new();
    protected string? ErrorMessage;
    protected bool IsRegistering;
    public string? DisplayToast { get; set; } = "d-block";

    [Inject]
    private RefreshTokenService RefreshTokenService { get; set; }
    [Inject]
    protected IJWTService JWTService { get; set; }
    [Inject]
    protected ITOTPService TotpService { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    protected ILHostService LHostService { get; set; }
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private IAuthService AuthService { get; set; }
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
                oTPModel.Email = result.Value;
            }
            if (string.IsNullOrEmpty(oTPModel.Email))
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
            var result = await AuthService.VerifyTOTPAsync(oTPModel);
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
