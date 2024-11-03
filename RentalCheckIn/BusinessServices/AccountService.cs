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
        // Check if user is blocked
        else if (lHost.IsBlockedSince.HasValue)
        {
            // Lift the block after 15 minutes
            TimeSpan blockDuration = TimeSpan.FromMinutes(15);
            var remainingTime = blockDuration - (DateTime.UtcNow - lHost.IsBlockedSince.Value);

            if (remainingTime > TimeSpan.Zero)
            {
                // User is still blocked
                string formattedRemainingTime = $"{remainingTime.Minutes} minutes and {remainingTime.Seconds} seconds";

                return new AuthenticationResult
                {
                    Success = false,
                    Message = $"Your account is blocked. Please wait for {formattedRemainingTime} before trying again."
                };
            }
            else
            {
                // Unblock the user if the block duration has passed
                lHost.IsBlockedSince = null;
                lHost.LoginAttempts = 0;
            }
        }
        else if (!VerifyPassword(hostLoginDto.Password, lHost.PasswordHash) && lHost.EmailConfirmed)
        {
            // Increase login attempts for unsuccessful logins
            await hostRepository.UpdateLHostPartialAsync(lHost, host =>
            {
                host.LoginAttempts += 1;
                // Block user if login attempts exceed limit
                if (lHost.LoginAttempts > 5)
                {
                    host.IsBlockedSince = DateTime.UtcNow;
                }
            });

            return new AuthenticationResult
            {
                Success = false,
                Message = "Invalid email or password, please try again"
            };

        }
        else if (!lHost.EmailConfirmed)
        {
            // Increase login attempts for unsuccessful logins
            await hostRepository.UpdateLHostPartialAsync(lHost, host =>
            {
                host.LoginAttempts += 1;
                // Block user if login attempts exceed limit
                if (lHost.LoginAttempts > 5)
                {
                    host.IsBlockedSince = DateTime.UtcNow;
                }
            });

            return new AuthenticationResult
            {
                Success = false,
                Message = "The verification link has been sent to your email, please check your spam folder."
            };
        }

        // Determine what to do with the bool return type
        await hostRepository.UpdateLHostPartialAsync(lHost, host =>
        {
            host.LastLogin = DateTime.Now;
            // Reset login attempt to zero for a successful login
            host.LoginAttempts = 0;
        });

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
        var encodedToken = HttpUtility.UrlEncode(lHost.EmailVerificationToken);
        // Send the new verification email
        var verificationLink = $"{configuration["ApplicatioSettings:AppUrl"]}/email-confirmation?token={encodedToken}";
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
                // Invalid request
                Message = "Invalid request"
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

        // Determine what to do with the bool return type
        await hostRepository.UpdateLHostPartialAsync(lHost, host =>
        {
            // Confirm the email and clear the token
            host.EmailConfirmed = true;
            // Invalidate the token after use
            host.EmailVerificationToken = null;
            // There's no token stored in the db
            host.EmailVTokenExpiresAt = default;
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
