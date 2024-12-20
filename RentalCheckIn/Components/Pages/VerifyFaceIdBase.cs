﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Components.Pages;

public class VerifyFaceIdBase : ComponentBase
{
    protected string? ErrorMessage { get; set; }
    protected bool IsAuthenticating { get; set; }
    protected bool ShouldDisplayRetryButton { get; set; }

    protected string? DisplayToast { get; set; } = "d-block";
    [Inject]
    private IAuthUIService AuthService { get; set; }
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private ILogger<VerifyFaceIdBase> Logger { get; set; }

    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Authenticate();
    }

    protected async Task Authenticate()
    {

        try
        {
            IsAuthenticating = true;
            ErrorMessage = string.Empty;
            var result = await AuthService.AuthenticateFaceIdAsync();

            if (result.IsSuccess)
            {
                ShouldDisplayRetryButton  = false;
                await LocalStorage.DeleteAsync("UserIdFor2FA");
            }
            else 
            {
                ShouldDisplayRetryButton  = true;
                ErrorMessage = result.Message;
            }
        }
        catch(Exception ex) 
        {
            ErrorMessage = Localizer["UnexpectedErrorOccurred"];
            Logger.LogError(ex, "An unexpected error occurred while trying to verify face Id in VerifyFaceIdComponent.");
        }
        finally
        {
            IsAuthenticating = false;
        }
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
