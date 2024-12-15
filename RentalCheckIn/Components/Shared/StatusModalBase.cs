using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Shared;
public class StatusModalBase : ComponentBase
{
    [Parameter]
    public bool IsOpen { get; set; }
    [Parameter]
    public bool IsSuccess { get; set; }
    [Parameter]
    public string? Message {  get; set; }    
    [Parameter]
    public EventCallback OnClose { get; set; }

    protected string ModalVisibility => IsOpen ? "show d-block" : "d-none";

    protected async Task Close()
    {
        await OnClose.InvokeAsync();
    }
}
