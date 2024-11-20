using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;
public class HomeBase : ComponentBase
{
    protected uint currentPage = 1;
    protected uint itemsPerPage;
    protected string Message;
    protected bool showModal = false;
    protected ReservationDTO selectedReservation;
    protected string SelectedApartment { get; set; } = "All";
    protected List<string> ApartmentNames = new List<string>();
    protected List<ReservationDTO> Reservation = new List<ReservationDTO>();
    protected List<Setting> Settings = new List<Setting>();

    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private IReservationService ReservationService { get; set; }
    [Inject]
    private IAppartmentService AppartmentService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Reservation = (await ReservationService.GetAllTableReservationsAsync()).ToList();
            Settings = (await ReservationService.GetSettingsAsync()).ToList();
            // Extract distinct apartment names for the dropdown
            ApartmentNames = (await AppartmentService.GetDistinctAppartmentNames()).ToList();

            // Determine items per page from settings
            if (Settings != null && Settings.Any())
            {
                itemsPerPage = Settings[0].RowsPerPage;
            }
            else
            {
                // Default value if settings are missing
                itemsPerPage = 10;
            }

            // Add 'All' option to allow viewing all apartments
            ApartmentNames.Insert(0, "All");
        }
        catch (Exception ex)
        {
            Message = "Could not load resources.";
        }
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
            Message = "An unexpected error occurred.";
        }
    }


    // Computes the total number of pages based on filtered reservations.
    protected uint totalPages => (uint)Math.Ceiling((double)FilteredReservations.Count() / itemsPerPage);

    // Filters the reservations based on the selected apartment.
    protected IEnumerable<ReservationDTO> FilteredReservations
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

    // Retrieves the reservations for the current page after applying the filter.
    protected IEnumerable<ReservationDTO> PaginatedReservations => FilteredReservations
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

    // Handles changes to the apartment filter.
    // Resets the current page to 1 and triggers a UI update.
    protected void OnApartmentFilterChanged(ChangeEventArgs e)
    {
        SelectedApartment = e.Value?.ToString() ?? "All";
        currentPage = 1;
    }

    protected void OpenModal(ReservationDTO reservation)
    {
        selectedReservation = reservation;
        showModal = true;
    }

    protected void CloseModal()
    {
        showModal = false;
        selectedReservation = null;
    }
}
