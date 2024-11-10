using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class HomeBase : ComponentBase
{
    protected uint currentPage = 1;
    protected uint itemsPerPage;
    protected string SelectedApartment { get; set; } = "All";
    protected List<string> ApartmentNames = new List<string>();
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
        // Determine items per page from settings
        if (Settings != null && Settings.Any())
        {
            itemsPerPage = Settings[0].RowsPerPage;
        }
        else
        {
            itemsPerPage = 10; // Default value if settings are missing
        }

        // Extract distinct apartment names for the dropdown
        ApartmentNames = Reservation
                         .Select(r => r.ApartmentName)
                         .Distinct()
                         .OrderBy(name => name)
                         .ToList();

        // Add 'All' option to allow viewing all apartments
        ApartmentNames.Insert(0, "All");
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

    /// <summary>
    /// Computes the total number of pages based on filtered reservations.
    /// </summary>
    protected uint totalPages => (uint)Math.Ceiling((double)FilteredReservations.Count() / itemsPerPage);


    /// <summary>
    /// Filters the reservations based on the selected apartment.
    /// </summary>
    protected IEnumerable<ReservationDto> FilteredReservations
    {
        get
        {
            if (SelectedApartment == "All")
            {
                return Reservation;
            }
            else
            {
                return Reservation.Where(r => r.ApartmentName == SelectedApartment);
            }
        }
    }
    /// <summary>
    /// Retrieves the reservations for the current page after applying the filter.
    /// </summary>
    protected IEnumerable<ReservationDto> PaginatedReservations => FilteredReservations
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

    /// <summary>
    /// Handles changes to the apartment filter.
    /// Resets the current page to 1 and triggers a UI update.
    /// </summary>
    /// <param name="newApartment">The newly selected apartment.</param>
    protected void OnApartmentFilterChanged(ChangeEventArgs e)
    {
        SelectedApartment = e.Value?.ToString() ?? "All";
        currentPage = 1;
    }

}
