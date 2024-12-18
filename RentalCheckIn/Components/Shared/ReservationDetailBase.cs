using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Shared;

public class ReservationDetailBase : ComponentBase
{
    [Parameter]
    public ReservationDTO SelectedReservation { get; set; }
    [Parameter]
    public EventCallback<uint> OnOpenCheckInDocument { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public void NavigateToCheckIn()
    {
        if (SelectedReservation?.StatusId == (uint)ReservationStatus.Okay)
        {
            NavigationManager.NavigateTo($"/checkin/{SelectedReservation.Id}", forceLoad: true);
        }
    }

    // Open the currently selected reservation/guest's check-in document
    protected async Task OpenCheckInDocument()
    {
        if (OnOpenCheckInDocument.HasDelegate && SelectedReservation?.CheckedInAt != null)
        {
            await OnOpenCheckInDocument.InvokeAsync(SelectedReservation.Id);
        }
    }

}
