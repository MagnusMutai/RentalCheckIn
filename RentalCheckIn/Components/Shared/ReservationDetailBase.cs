using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Shared;

public class ReservationDetailBase : ComponentBase
{
    protected bool showModal = false;
    [Parameter]
    public ReservationDTO SelectedReservation { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public void NavigateToCheckIn()
    {
        if (SelectedReservation != null)
        {
            NavigationManager.NavigateTo($"/checkin/{SelectedReservation.Id}");
        }
    }
}
