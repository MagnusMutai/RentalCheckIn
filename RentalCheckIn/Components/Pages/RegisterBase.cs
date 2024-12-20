using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using QRCoder;
using RentalCheckIn.Enums;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Components.Pages;
public class RegisterBase : ComponentBase
{
    protected HostSignUpDTO RegisterModel { get; set; } = new();
    protected string? Message { get; set; }
    protected string? TOTPSecret { get; set; }
    protected bool IsRegistering { get; set; }
    protected bool IsRegistrationComplete { get; set; }
    public string? DisplayToast { get; set; } = "d-block";
    public string BackGroundColor { get; set; } = "bg-success";
    // Do we need to use this again in the UI or can we use it only once in the base class?
    protected bool IsFaceIdSelected => RegisterModel.AuthenticatorId == AuthenticatorType.FACEID;
    // To store the LHost ID after registration
    protected uint RegisteredLHostId { get; set; } 
    protected string? QrCodeImageData { get; set; }
    [Inject]
    protected IAuthUIService AuthService { get; set; }
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

            var result = await AuthService.RegisterAsync(RegisterModel);

            if (result.IsSuccess)
            {
                RegisteredLHostId = result.Data.HostId;
                BackGroundColor = "bg-success";
                // Use best practices for localization keys(.)
                Message = Localizer["RegisterAccountCreatedSuccessfully"];

                // Mark registration as complete
                IsRegistrationComplete = true;

                if (!IsFaceIdSelected)
                {
                    TOTPSecret = result.Data.TOTPSecret;

                    // Generate the otpauth URI
                    string issuer = "RentalCheckIn"; // Replace with your app's name
                    string account = RegisterModel.Email;
                    string otpauthUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(account)}?secret={TOTPSecret}&issuer={Uri.EscapeDataString(issuer)}";

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

            IsRegistering = false;
        }
        catch (Exception ex)
        {
            BackGroundColor = "bg-danger";
            Message = Localizer["UnexpectedErrorOccurred"];
            IsRegistering = false;
            Logger.LogError(ex, "An unexpected error occurred while trying to register a user.");
        }
        finally
        {
            DisplayToast = DisplayToast ?? "d-block";
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

        DisplayToast = DisplayToast ?? "d-block";
    }
    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
