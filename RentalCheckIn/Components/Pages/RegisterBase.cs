using Microsoft.AspNetCore.Components;
using QRCoder;

namespace RentalCheckIn.Components.Pages;
public class RegisterBase : ComponentBase
{
    protected HostSignUpDto registerModel = new();
    protected string ErrorMessage;
    protected string SuccessMessage;
    protected string TotpSecret;
    protected bool ShouldSpin;
    public string DisplayToast { get; set; } = "d-block";
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
        ShouldSpin = true;
        var result = await AuthService.RegisterAsync(registerModel);

        if (result.Success)
        {
            TotpSecret = result.Host.TotpSecret;

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


            SuccessMessage = "Your account was created. An account confirmation link has been sent to your email.";
        }
        ErrorMessage = result.Message;
        DisplayToast = DisplayToast ?? "d-block";
        ShouldSpin = false;
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
