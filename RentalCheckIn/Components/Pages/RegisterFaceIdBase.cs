using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class RegisterFaceIdBase : ComponentBase
{
    protected string? message;
    protected bool isRegistering;
    private bool isRegisteringOnce;
    protected bool shouldDisplayRegButton;


    [Inject]
    protected IAuthService AuthService { get; set; }

    [Parameter]
    public uint HostId { get; set; }

    [Parameter]
    public EventCallback<OperationResult> OnRegistrationComplete { get; set; }
    public OperationResult RegistrationResult { get; set; }
    [Inject]
    private ILogger<RegisterFaceIdBase> Logger { get; set; }


    protected override async Task OnInitializedAsync()
    {
        if (!isRegisteringOnce)
        {
            isRegisteringOnce = true;
            await HandleRegisterFaceId();
        }
    }

    protected async Task HandleRegisterFaceId()
    {
        try
        {
            isRegistering = true;
            message = string.Empty;
            var result = await AuthService.RegisterFaceIdAsync(HostId);
            RegistrationResult = result;
            message = result.Message;

            await OnRegistrationComplete.InvokeAsync(result);

            shouldDisplayRegButton = !RegistrationResult.IsSuccess;
        }
        catch(Exception ex)
        {
            message = "An unexpected error occurred during Face ID registration.";
            RegistrationResult = new OperationResult
            {
                IsSuccess = false,
                Message = "An unexpected error occurred during Face ID registration."
            };

            await OnRegistrationComplete.InvokeAsync(RegistrationResult);

            Logger.LogError(ex, "An unexpected error occurred while trying to register face Id in RegisterFaceId component.");
        }
        finally
        {
            isRegistering = false;
        }
    }
}
