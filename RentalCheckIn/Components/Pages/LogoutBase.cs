﻿using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class LogoutBase : ComponentBase
{
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    protected NavigationManager NavigationManager{ get; set; }

    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Delete email for TOTP
        await LocalStorage.DeleteAsync("emailForOtp");
        // Mark user as logged out
        await LocalStorage.DeleteAsync("token");
        Constants.JWTToken = "";
        await AuthStateProvider.GetAuthenticationStateAsync();

        // Notify the authentication state provider
        if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
        {
            await customAuthStateProvider.NotifyUserLogout();
        }

        // Redirect to login page
        NavigationManager.NavigateTo("/login");
    }
}
