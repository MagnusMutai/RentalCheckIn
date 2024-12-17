using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
namespace RentalCheckIn.Components.Pages;

public class RequestPasswordResetBase : ComponentBase
{
    protected ResetRequestDTO ResetModel { get; set; } = new ResetRequestDTO();
    protected string? Message { get; set; }
    protected bool IsRegistering { get; set; }
    public string? DisplayToast { get; set; } = "d-block";
    public string BackGroundColor { get; set; } = "bg-success";

    [Inject]
    protected IAuthService AuthService { get; set; }
    [Inject]
    private ILogger<RequestPasswordResetBase> Logger { get; set; }

    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }

    protected async Task HandleValidSubmit()
    {
        try
        {
            IsRegistering = true;
            var result = await AuthService.ForgotPasswordAsync(ResetModel);

            // In other similar pages remember to go back and check null first.
            if (result != null)
            {
                if (result.IsSuccess)
                {
                    BackGroundColor = "bg-success";
                    Message = result.Message;
                }
                else
                {
                    BackGroundColor = "bg-danger";
                    Message = result.Message;
                }
            }

            IsRegistering = false;
        }
        catch (Exception ex) 
        {
            BackGroundColor = "bg-danger";
            Message = Localizer["UnexpectedErrorOccurred"];
            Logger.LogError(ex, "An unexpected error occurred while requeting password reset in RequestPasswordReset component");
        }
        finally
        {
            DisplayToast = DisplayToast ?? "d-block";
        }
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
