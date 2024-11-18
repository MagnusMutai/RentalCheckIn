using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class VerifyTOTPBase : ComponentBase
{
    protected TOTPDTO oTPModel = new();
    protected string ErrorMessage;
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var result = await LocalStorage.GetAsync<string>("emailForOtp");

        if (result.Success)
        {
            oTPModel.Email = result.Value;
        }
        if (string.IsNullOrEmpty(oTPModel.Email))
        {
            NavigationManager.NavigateTo("/login");
        }
    }

    protected async Task HandleVerifyOtp()
    {
        IsRegistering = true;
        try
        {
            var result = await AuthService.VerifyTOTPAsync(oTPModel);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An unexpected error has occurred. Please try again later.";
        }
        IsRegistering = false;
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }


}
