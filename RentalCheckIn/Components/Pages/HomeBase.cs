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
            Constants.JWTToken = response.Success ? response.Value : "";

            if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
            {
                var authState = await customAuthStateProvider.NotifyUserAuthentication(Constants.JWTToken);
                if (authState.User.Identity is { IsAuthenticated: false })
                {
                    // User is not authenticated; redirect to login
                    NavigationManager.NavigateTo("/login", forceLoad: false);
                }
            }


        }
        catch (Exception ex)
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
