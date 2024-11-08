using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class RegisterBase : ComponentBase
{
    protected HostSignUpDto registerModel = new();
    protected string ErrorMessage;
    protected string SuccessMessage;
    protected string TotpSecret;
    protected bool ShouldSpin;
    public string DisplayToast { get; set; } = "d-block";
    [Inject]
    protected IAuthService AuthService { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    protected override async Task OnInitializedAsync()
    {
        // Get the current authentication state
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        // Check if the user is authenticated
        if (authState.User.Identity is { IsAuthenticated: true })
        {
            // Redirect to the home page if the user is already logged in
            NavigationManager.NavigateTo("/");
        }
    }

    protected async Task HandleRegister()
    {
        ShouldSpin = true;
        var result = await AuthService.RegisterAsync(registerModel);

        if (result.Success)
        {
            TotpSecret = result.Host.TotpSecret;
            SuccessMessage = "Your account was created. An account confirmation link has been sent to your email.";
        }
        ErrorMessage = result.Message;
        DisplayToast = DisplayToast ?? "d-block";
        ShouldSpin = false;
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
