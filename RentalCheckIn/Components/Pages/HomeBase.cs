using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
using System.Globalization;
namespace RentalCheckIn.Components.Pages;
public class HomeBase : ComponentBase
{
    protected uint currentPage = 1;
    protected uint itemsPerPage;
    protected string? Message;
    protected bool showModal = false;
    protected ReservationDTO? selectedReservation;
    protected string SelectedApartment { get; set; } = "All";
    protected List<string> ApartmentNames = new List<string>();
    protected List<ReservationDTO> Reservations = new List<ReservationDTO>();
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
    private ILocalizationUIService LocalizationUIService { get; set; }
    [Inject]
    private ILogger<HomeBase> Logger { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }
    protected override async Task OnInitializedAsync()
    {
        try
        {
            // Fetch reservations
            // Implement Result pattern in future
            var reservationResult = (await ReservationService.GetAllTableReservationsAsync()).ToList();
            if (reservationResult != null)
            {
                Reservations = reservationResult;
            }

            // Fetch settings
            // Implement Result pattern in future
            var settingsResult = (await ReservationService.GetSettingsAsync()).ToList();
            if (settingsResult != null)
            {
                Settings = settingsResult;
            }

            // Determine items per page from settings
            if (Settings != null && Settings.Any() && Settings[0].RowsPerPage > 0)
            {
                itemsPerPage = Settings[0].RowsPerPage;
            }
            else
            {
                // Default value if settings are missing
                itemsPerPage = 10;
            }

            await LoadLocalizedDataAsync();
        }
        catch (Exception ex)
        {
            Message = Localizer["CouldNotLoadResources"];
            Logger.LogError(ex, "An unexpected error occurred while trying to load reservations or its dependencies in the Reservation Page.");
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
                var authState = customAuthStateProvider.NotifyUserAuthentication(Constants.JWTToken);

                if (authState.User.Identity is { IsAuthenticated: false })
                {
                    // User is not authenticated; redirect to login
                    NavigationManager.NavigateTo("/login", forceLoad: false);
                }
            }

            // Update default value with localized text
            if (SelectedApartment == "All" && Localizer != null)
            {
                SelectedApartment = Localizer["All"];
            }
        }
        catch (Exception ex)
        {
            Message = Localizer["UnexpectedErrorOccurred"];
            Logger.LogError(ex, "An unexpected error occurred either while trying to authenticate a user or assigning localized apartment names on Reservation page.");
        }
    }

    private async Task LoadLocalizedDataAsync()
    {
        try
        {
            // Fetch localized apartment names and status labels
            var apartmentIds = Reservations.Select(r => r.ApartmentId).Distinct();
            var statusIds = Reservations.Select(r => r.StatusId).Distinct();
            var culture = CultureInfo.CurrentCulture.Name;
            var apartmentNamesDict = await LocalizationUIService.GetApartmentNamesAsync(apartmentIds, culture);
            var statusLabelsDict = await LocalizationUIService.GetStatusLabelsAsync(statusIds, culture);

            // Assign localized names to reservations
            foreach (var reservation in Reservations)
            {
                reservation.ApartmentName = apartmentNamesDict.ContainsKey(reservation.ApartmentId)
                    ? apartmentNamesDict[reservation.ApartmentId]
                    : "[Apartment Name]";

                reservation.StatusLabel = statusLabelsDict.ContainsKey(reservation.StatusId)
                    ? statusLabelsDict[reservation.StatusId]
                    : "[Status Label]";
            }

            // Extract distinct apartment names for the dropdown
            ApartmentNames = Reservations.Select(r => r.ApartmentName).Distinct().ToList();

            // Add 'All' option to allow viewing all apartments
            ApartmentNames.Insert(0, Localizer["All"]);
        }
        catch (Exception ex)
        {
            Message = Localizer["CouldNotLoadResources"];
            Logger.LogError(ex, "An error occurred while loading localized data.");
        }
    }

    // Computes the total number of pages based on filtered reservations.
    protected uint totalPages => (uint)Math.Ceiling((double)FilteredReservations.Count() / itemsPerPage);

    // Filters the reservations based on the selected apartment.
    protected IEnumerable<ReservationDTO> FilteredReservations
    {
        get
        {
            if (SelectedApartment == Localizer["All"])
            {
                return Reservations;
            }
            else
            {
                return Reservations.Where(r => r.ApartmentName == SelectedApartment);
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
        SelectedApartment = e.Value?.ToString() ?? Localizer["All"];
        currentPage = 1;
    }

    protected void OpenModal(ReservationDTO reservation)
    {
        if (reservation != null)
        {
            selectedReservation = reservation;
            showModal = true;
        }
    }

    protected void CloseModal()
    {
        showModal = false;
        selectedReservation = null;
    }
}
