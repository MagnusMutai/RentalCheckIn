using Microsoft.AspNetCore.Components;
using System.Linq;

namespace RentalCheckIn.Components.Pages;
public class HomeBase : ComponentBase
{
    protected uint currentPage = 1;
    protected uint itemsPerPage;

    protected List<ReservationDto> Reservation = new List<ReservationDto>();
    protected List<Setting> Settings = new List<Setting>();

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
        Settings = (await ReservationService.GetSettingsAsync()).ToList();
        itemsPerPage = Settings[0].RowsPerPage;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
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
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    protected uint totalPages => (uint)Math.Ceiling((double)Reservation?.Count() / itemsPerPage);

    protected IEnumerable<ReservationDto> PaginatedReservations => Reservation
        .Skip((int)((currentPage - 1) * itemsPerPage))
        .Take((int)itemsPerPage);

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

    protected void GoToPage(uint pageNumber)
    {
        if (pageNumber <= totalPages)
        {
            currentPage = pageNumber;
        }
    }

}
