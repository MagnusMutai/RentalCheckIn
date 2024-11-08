using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class HomeBase : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Get the current authentication state
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        // Check if the user is authenticated
        if (authState.User.Identity is { IsAuthenticated: false })
        {
            // Redirect to the home page if the user is already logged in
            NavigationManager.NavigateTo("/login");
        }
    }
}
