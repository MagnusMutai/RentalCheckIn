﻿using Microsoft.AspNetCore.Components;
using QRCoder;

namespace RentalCheckIn.Components.Pages;
public class RegisterBase : ComponentBase
{
    protected HostSignUpDTO registerModel = new();
    protected string Message;
    protected string TotpSecret;
    protected bool ShouldSpin;
    public string DisplayToast { get; set; } = "d-block";
    public string BackGroundColor { get; set; } = "bg-success";
    protected string QrCodeImageData { get; set; }
    [Inject]
    protected IAuthService AuthService { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    protected override async Task OnInitializedAsync()
    {
        // Get the current authentication state
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        // Check if the user is authenticated
        if (authState.User.Identity is { IsAuthenticated: true })
        {
            // Redirect to the home page if the user is already logged in
            NavigationManager.NavigateTo("/");
        }
    }

    protected async Task HandleRegister()
    {
        try
        {
            ShouldSpin = true;
            var result = await AuthService.RegisterAsync(registerModel);

            if (result.IsSuccess)
            {
                TotpSecret = result.Data.TotpSecret;

                // Generate the otpauth URI
                string issuer = "RentalCheckIn"; // Replace with your app's name
                string account = registerModel.Email;
                string otpauthUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(account)}?secret={TotpSecret}&issuer={Uri.EscapeDataString(issuer)}";

                // Generate the QR code image
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(otpauthUri, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeAsPng = qrCode.GetGraphic(20);

                // Convert the image to a base64 string
                QrCodeImageData = "data:image/png;base64," + Convert.ToBase64String(qrCodeAsPng);
                BackGroundColor = "bg-success";
                Message = "Your account was created. An account confirmation link has been sent to your email.";
            }
            else
            {
                BackGroundColor = "bg-danger";
                Message = result.Message;
            }
            DisplayToast = DisplayToast ?? "d-block";
            ShouldSpin = false;
        }
        catch (Exception ex)
        {
            BackGroundColor = "bg-danger";
            Message = "An unexpected error has occurred. Please try again later";
        }
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
