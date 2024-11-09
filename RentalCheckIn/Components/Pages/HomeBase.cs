using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class HomeBase : ComponentBase
{
    protected int currentPage = 1;
    protected int itemsPerPage = 5; // Set the number of items per page

    protected List<ReservationDto> Reservation = new List<ReservationDto>();

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
        Reservation = (await ReservationService.GetAllReservationsAsync()).ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Get the accessToken if it exists
        var response = await LocalStorage.GetAsync<string>("token");
        if (response.Success)
        {
            Constants.JWTToken = response.Value;
        }
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        // Check if the user is authenticated
        if (authState.User.Identity is { IsAuthenticated: false })
        {
            NavigationManager.NavigateTo("/login", forceLoad: true);
        }
    }


    protected int totalPages => (int)Math.Ceiling((double)Reservation?.Count() / itemsPerPage);

    protected IEnumerable<ReservationDto> PaginatedReservations => Reservation
        .Skip((currentPage - 1) * itemsPerPage)
        .Take(itemsPerPage);

    protected void NextPage()
    {
        if (currentPage < totalPages)
        {
            currentPage++;
        }
    }

    protected void PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
        }
    }

    protected void GoToPage(int pageNumber)
    {
        if (pageNumber > totalPages)
            throw new Exception("Nonsense");
        currentPage = pageNumber;
    }

}
