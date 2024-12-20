using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Components.Pages;
public class RegisterFaceIdBase : ComponentBase
{
    protected string? Message { get; set; }
    protected bool IsRegistering { get; set; }
    private bool IsRegisteredOnce { get; set; }
    protected bool ShouldDisplayRegButton { get; set; }


    [Inject]
    protected IAuthUIService AuthService { get; set; }

    [Parameter]
    public uint HostId { get; set; }

    [Parameter]
    public EventCallback<OperationResult> OnRegistrationComplete { get; set; }
    public OperationResult RegistrationResult { get; set; }
    [Inject]
    private ILogger<RegisterFaceIdBase> Logger { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }


    protected override async Task OnInitializedAsync()
    {
        if (!IsRegisteredOnce)
        {
            IsRegisteredOnce = true;
            await HandleRegisterFaceId();
        }
    }

    protected async Task HandleRegisterFaceId()
    {
        try
        {
            IsRegistering = true;
            Message = string.Empty;
            var result = await AuthService.RegisterFaceIdAsync(HostId);
            RegistrationResult = result;
            Message = result.Message;

            await OnRegistrationComplete.InvokeAsync(result);

            ShouldDisplayRegButton = !RegistrationResult.IsSuccess;
        }
        catch(Exception ex)
        {
            Message = Localizer["FaceID.Registration.UnexpectedError"];
            RegistrationResult = new OperationResult
            {
                IsSuccess = false,
                Message = Localizer["FaceID.Registration.UnexpectedError"]
            };

            await OnRegistrationComplete.InvokeAsync(RegistrationResult);

            Logger.LogError(ex, "An unexpected error occurred while trying to register face Id in RegisterFaceId component.");
        }
        finally
        {
            IsRegistering = false;
        }
    }
}
