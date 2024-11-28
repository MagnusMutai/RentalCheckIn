using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Shared;
public class StackedReservationViewBase : ComponentBase
{
    [Parameter]
    public IEnumerable<ReservationDTO> PaginatedReservations { get; set; }
    [Parameter]
    public EventCallback<ReservationDTO> OnOpenModal { get; set; }

    protected async Task OpenModal(ReservationDTO reservation)
    {
        if (OnOpenModal.HasDelegate)
        {
            await OnOpenModal.InvokeAsync(reservation);
        }
    }
}
