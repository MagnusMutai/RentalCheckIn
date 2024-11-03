using Microsoft.EntityFrameworkCore;
using RentalCheckIn.Services;
using System.Security.Cryptography;
using System.Web;
using static RentalCheckIn.Responses.CustomResponses;

namespace RentalCheckIn.BusinessServices;
public class AccountService : IAccountService
{
    private readonly IHostRepository hostRepository;
    private readonly IConfiguration configuration;
    private readonly IEmailService emailService;

    public AccountService(IHostRepository hostRepository, IConfiguration configuration, IEmailService emailService)
    {
        this.hostRepository = hostRepository;
        this.configuration = configuration;
        this.emailService = emailService;
    }
    public async Task<AuthenticationResult> LoginAsync(HostLoginDto hostLoginDto)
    {
        var lHost = await hostRepository.GetByEmailAsync(hostLoginDto.Email);
        if (lHost == null)
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = "The account is not registered"
            };
        }
        
        if (!VerifyPassword(hostLoginDto.Password, lHost.PasswordHash))
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = "Invalid email or password, please try again"
            };
        }

        if (!lHost.EmailConfirmed)
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = "Check your email to confirm your account."
            };
        }

        return new AuthenticationResult
        {
            Success = true,
            Host = lHost,
        };
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task<AuthenticationResult> RegisterAsync(HostSignUpDto hostSignUpDto)
    {
        var existingHost = await hostRepository.GetByEmailAsync(hostSignUpDto.Email);
        if (existingHost != null)
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = "User already exists"
            };
        }

        var totpService = new TotpService();
        var totpSecret = totpService.GenerateSecret();

        var lHost = new Lhost
        {
            FirstName = hostSignUpDto.FirstName,
            LastName = hostSignUpDto.LastName,
            PasswordHash = HashPassword(hostSignUpDto.Password),
            TotpSecret = totpSecret,
            Username = hostSignUpDto.Email,
            MailAddress = hostSignUpDto.Email,
            EmailVerificationToken = GenerateEmailVerificationToken(),
            EmailVTokenExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        await hostRepository.AddLHostAsync(lHost);

        // Return the TOTP secret to the caller to display to the user
        lHost.TotpSecret = totpSecret;
        //var encodedToken = HttpUtility.UrlEncode(lHost.EmailVerificationToken);
        // Send the new verification email
        var verificationLink = $"{configuration["ApplicatioSettings:AppUrl"]}/email-confirmation?token={lHost.EmailVerificationToken}";
        await emailService.SendEmailAsync(lHost.MailAddress, "Confirm your email", $"Please confirm your email by clicking <a href=\"{verificationLink}\">here</a>.");

        return new AuthenticationResult
        {
            Success = true,
            Host = lHost
        };
    }

    public async Task<EmailVerificationResult> VerifyEmailTokenAsync(string token)
    {
        // Retrieve user by token from the repository
        var lHost = await hostRepository.GetUserByEmailVerificationTokenAsync(token);

        // Check if user or token is invalid
        // Counter-check the purpose of the logic
        if (lHost == null)
        {
            return new EmailVerificationResult
            {
                IsSuccess = false,
                Message = "Invalid or expired token."
            };
        }

        // Check if the token has expired
        if (lHost.EmailVTokenExpiresAt < DateTime.UtcNow)
        {
            return new EmailVerificationResult
            {
                IsSuccess = false,
                IsExpired = true,
                Message = "The verification link has expired. Please request a new verification email."
            };
        }

        // Check if the email is already confirmed
        if (lHost.EmailConfirmed)
        {
            return new EmailVerificationResult
            {
                IsSuccess = false,
                IsAlreadyConfirmed = true,
                Message = "This email has already been confirmed. Please log in."
            };
        }
        // Determine what to do with the return types
        await hostRepository.UpdateLHostPartialAsync(lHost, host =>
        {
            // Confirm the email and clear the token
            host.EmailConfirmed = true;
            // Invalidate the token after use
            host.EmailVerificationToken = null;  
        });

        return new EmailVerificationResult
        {
            IsSuccess = true,
            Message = "Email confirmed successfully."
        };
    }


    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public string GenerateEmailVerificationToken()
    {
        // 256 bits of randomness
        byte[] randomBytes = new byte[32]; 
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        // Unique token
        return Convert.ToBase64String(randomBytes); 
    }

}
