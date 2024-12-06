using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using QRCoder;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Components.Pages;
public class RegisterBase : ComponentBase
{
    protected HostSignUpDTO registerModel = new();
    protected string? Message;
    protected string? TotpSecret;
    protected string TOTP = "TOTP";
    protected string FaceID = "FaceID";
    protected bool IsRegistering;
    protected bool isRegistrationComplete;
    public string? DisplayToast { get; set; } = "d-block";
    public string BackGroundColor { get; set; } = "bg-success";
    protected bool IsFaceIdSelected => registerModel.Selected2FA == "FaceID";
    protected uint RegisteredUserId { get; set; } // To store the user ID after registration

    protected string? QrCodeImageData { get; set; }
    [Inject]
    protected IAuthService AuthService { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private ILogger<RegisterBase> Logger { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set;}
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
            IsRegistering = true;

            var result = await AuthService.RegisterAsync(registerModel);

            if (result.IsSuccess)
            {
                RegisteredUserId = result.Data.HostId;
                BackGroundColor = "bg-success";
                Message = Localizer["RegisterAccountCreatedSuccessfully"];

                // Mark registration as complete
                isRegistrationComplete = true;

                if (!IsFaceIdSelected)
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
                    Message = Localizer["AccountCreatedConfirmationEmailSent"];
                }
            }
            else
            {
                BackGroundColor = "bg-danger";
                Message = result.Message;
            }
            DisplayToast = DisplayToast ?? "d-block";
            IsRegistering = false;
        }
        catch (Exception ex)
        {
            BackGroundColor = "bg-danger";
            Message = Localizer["UnexpectedErrorOccurred"];
            DisplayToast = "d-block";
            IsRegistering = false;
            Logger.LogError(ex, "An unexpected error occurred while trying to register a user.");
        }
    }

    // Callback when Face ID registration is complete
    protected void HandleFaceIdRegistrationComplete(OperationResult result)
    {
        if (result.IsSuccess)
        {
            Message = Localizer["FaceID.Registration.Success"];
            BackGroundColor = "bg-success";
        }
        else
        {
            Message = $"{Localizer["FaceID.Registration.Failed"]}: {result.Message}";
            BackGroundColor = "bg-danger";
        }
        DisplayToast = "d-block";
    }
    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
