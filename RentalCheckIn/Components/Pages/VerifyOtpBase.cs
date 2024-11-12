using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class VerifyOtpBase : ComponentBase
{
    protected OtpDto otpModel = new();
    protected string ErrorMessage;
    protected bool ShouldSpin;

    [Inject]
    private RefreshTokenService RefreshTokenService { get; set; }
    [Inject]
    protected JwtService JwtService { get; set; }
    [Inject]
    protected TotpService TotpService { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    protected ILHostService LHostService { get; set; }
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var result = await LocalStorage.GetAsync<string>("emailForOtp");

        if (result.Success)
        {
            otpModel.Email = result.Value;
        }
        if (string.IsNullOrEmpty(otpModel.Email))
        {
            NavigationManager.NavigateTo("/login");
        }
    }

    protected async Task HandleVerifyOtp()
    {
        ShouldSpin = true;
        try
        {
            // This should be an api endpoint
            var lHost = await LHostService.GetLHostByEmail(otpModel.Email);
            if (lHost == null)
                throw new Exception("User not found");

            if (!TotpService.VerifyCode(lHost.TotpSecret, otpModel.Code))
                throw new Exception("Invalid OTP code");

            var refreshToken = await RefreshTokenService.GenerateRefreshToken(lHost.HostId);
            await LocalStorage.SetAsync("refreshToken", refreshToken.Token);
            var accessToken = JwtService.GenerateToken(lHost);
            Constants.JWTToken = accessToken;
            // Store the token in local storage
            await LocalStorage.SetAsync("token", accessToken);

            await AuthStateProvider.GetAuthenticationStateAsync();

            // Navigate to the home page or dashboard
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Invalid request";
        }
        ShouldSpin = false;
    }

}
