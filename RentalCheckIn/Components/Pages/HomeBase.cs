using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class HomeBase : ComponentBase
{

    protected List<Reservation> Reservation = new List<Reservation>();

    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private IReservationService ReservationService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Reservation = (await ReservationService.GetAllReservationsAsync()).ToList() ;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Get the accessToken if it exists
        var response = await LocalStorage.GetAsync<string>("token");
        if (response.Success)
        {
            Constants.JWTToken = response.Value;
        }

        // Get the current authentication state
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        // Check if the user is authenticated
        if (authState.User.Identity is { IsAuthenticated: false })
        {
            // Redirect to the home page if the user is already logged in
            NavigationManager.NavigateTo("/login", forceLoad: true);
        }
    }

}
