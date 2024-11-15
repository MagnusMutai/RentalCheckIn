﻿using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class LoginBase : ComponentBase
{
    protected HostLoginDTO loginModel = new();
    protected string ErrorMessage;
    protected bool ShouldSpin;
    public string? DisplayToast { get; set; } = "d-block";
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IAuthService AuthService { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Get the current authentication state
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        // Check if the user is authenticated
        if (authState.User.Identity is { IsAuthenticated: true } )
        {
            // Redirect to the home page if the user is already logged in
            NavigationManager.NavigateTo("/", forceLoad:true);
        }
    }

    protected async Task HandleLogin()
    {
        ShouldSpin = true;
        try
        {
            var result = await AuthService.LoginAsync(loginModel);
            if (result.IsSuccess)
            {
                var lHost = result.Data;
                // Store email for OTP verification
                await LocalStorage.SetAsync("emailForOtp", lHost.MailAddress);
                NavigationManager.NavigateTo("/verify-otp");
            }
            else
            {
                ErrorMessage = result.Message;
                DisplayToast = DisplayToast ?? "d-block";
            }
        }
        catch (Exception ex) 
        {
            ErrorMessage = "An unexpected error occurred";
        }
        ShouldSpin = false;
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }

}
