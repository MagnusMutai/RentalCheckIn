using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Shared;

public class ReservationDetailBase : ComponentBase
{
    protected bool showModal = false;
    [Parameter]
    public ReservationDTO SelectedReservation { get; set; }

}
