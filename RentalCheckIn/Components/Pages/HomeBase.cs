using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using RentalCheckIn.Locales;
using System.Globalization;
namespace RentalCheckIn.Components.Pages;
public class HomeBase : ComponentBase, IAsyncDisposable
{
    private IJSObjectReference? module;
    protected uint CurrentPage { get; set; } = 1;
    protected uint ItemsPerPage { get; set; }
    protected string? Message { get; set; }
    protected bool ShowModal { get; set; }
    protected ReservationDTO? SelectedReservation { get; set; }
    protected string SelectedApartment { get; set; } = "All";
    protected List<string> ApartmentNames { get; set; } = new List<string>();
    protected List<ReservationDTO> Reservations { get; set; } = new List<ReservationDTO>();
    protected List<Setting> Settings { get; set; } = new List<Setting>();

    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private IReservationUIService ReservationService { get; set; }
    [Inject]
    private ILocalizationUIService LocalizationUIService { get; set; }
    [Inject]
    private ILogger<HomeBase> Logger { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }
    [Inject]
    private IJSRuntime JSRuntime { get; set; }
    [Inject]
    private IDocumentUIService DocumentUIService { get; set; }
    // Computes the total number of pages based on filtered reservations.
    protected uint TotalPages
    {
        get
        {
            if (ItemsPerPage == 0)
                return 0;

            return (uint)Math.Ceiling((double)FilteredReservations.Count() / ItemsPerPage);
        }
    }
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
        .Skip((int)((CurrentPage - 1) * ItemsPerPage))
        .Take((int)ItemsPerPage);

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
                ItemsPerPage = Settings[0].RowsPerPage;
            }
            else
            {
                // Default value if settings are missing
                ItemsPerPage = 10;
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
            // Use JS modules
            // Load JS module from a collocated JS file.
            if (firstRender)
            {
                module = await JSRuntime.InvokeAsync<IJSObjectReference>("import",
                    "./Components/Pages/Home.razor.js");
            }            

            // Must we always OnAfterender Query the local storage? Or should we check the authentication state before make unnecessary queries?
            // Get the accessToken if it exists
            var response = await LocalStorage.GetAsync<string>("token");
            Constants.JWTToken = response.Success ? response.Value : "";

            if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
            {
                var authState = await customAuthStateProvider.GetAuthenticationStateAsync();

                if (authState.User.Identity is { IsAuthenticated: false })
                {
                    // User is not authenticated; redirect to login.
                    NavigationManager.NavigateTo("/login", forceLoad: false);
                }
            }

            // Update default value with localized text.
            if (SelectedApartment == "All" && Localizer != null)
            {
                SelectedApartment = Localizer["All"];
            }
        }
        catch (Exception ex)
        {
            Message = Localizer["UnexpectedErrorOccurred"];
            Logger.LogError(ex, "An unexpected error occurred while trying to authenticate a user, assigning modules to IJSObjectReferences or assigning localized apartment names on Reservation page.");
        }
    }

    private async Task LoadLocalizedDataAsync()
    {
        try
        {
            // Fetch localized apartment names and status labels.
            var apartmentIds = Reservations.Select(r => r.ApartmentId).Distinct();
            var statusIds = Reservations.Select(r => r.StatusId).Distinct();
            var culture = CultureInfo.CurrentCulture.Name;
            var apartmentNamesDict = await LocalizationUIService.GetApartmentNamesAsync(apartmentIds, culture);
            var statusLabelsDict = await LocalizationUIService.GetStatusLabelsAsync(statusIds, culture);

            // Assign localized names to reservations.
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
            Logger.LogError(ex, "An unexpected error occurred while loading localized data in HomeBase (LoadLocalizedDataAsync method).");
        }
    }

    protected void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
        }
    }

    protected void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
        }
    }

    protected void GoToPage(uint pageNumber)
    {
        if (pageNumber <= TotalPages)
        {
            CurrentPage = pageNumber;
        }
    }

    // Handles changes to the apartment filter.
    // Resets the current page to 1 and triggers a UI update.
    protected void OnApartmentFilterChanged(ChangeEventArgs e)
    {
        SelectedApartment = e.Value?.ToString() ?? Localizer["All"];
        CurrentPage = 1;
    }

    protected void OpenModal(ReservationDTO reservation)
    {
        if (reservation != null)
        {
            SelectedReservation = reservation;
            ShowModal = true;
        }
    }

    protected void CloseModal()
    {
        ShowModal = false;
        SelectedReservation = null;
    }

    protected async Task HandleDisplayDocument(uint reservationId)
    {
        try
        {
            var reservationData = await ReservationService.GetCheckInReservationByIdAsync(reservationId);
           
            if (reservationData == null)
            {
                Message = Localizer["Error.DocumentDisplay"];
            }
            else
            {
                var docRequest = new OperationRequest
                {
                    Culture = CultureInfo.CurrentCulture.Name,
                    Model = reservationData
                };

                var pdfByteArray = await DocumentUIService.GenerateCheckInFormAsync(docRequest);
                var pdfUri = $"data:application/pdf;base64,{Convert.ToBase64String(pdfByteArray?.Data)}";

                if (module is not null)
                {
                    // Add localization data for the popup
                    var enablePopupMessage = Localizer["Enable:Popup"].Value;
                    await module.InvokeVoidAsync("initializeLocalizationData", new { EnablePopupMessage = enablePopupMessage });
                    // Open the pdf document
                    await module.InvokeVoidAsync("openPDFDocument", pdfUri, enablePopupMessage);
                    //StateHasChanged();
                }
            }

        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred while displaying checkin pdf in HomeBase (HandleDisplayDocument method).");
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (module is not null)
        {
            try
            {
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException ex)
            {
                Logger.LogInformation(ex, "JavaScript runtime unexpectedly disconnected.");
            }
        }

    }
}
