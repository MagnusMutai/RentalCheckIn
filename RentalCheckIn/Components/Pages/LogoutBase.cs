using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class LogoutBase : ComponentBase
{
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    protected NavigationManager NavigationManager{ get; set; }

    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Mark user as logged out
        await LocalStorage.DeleteAsync("token");
        Constants.JWTToken = "";
        await AuthStateProvider.GetAuthenticationStateAsync();
        // Redirect to login page
        NavigationManager.NavigateTo("/login");
    }
}
