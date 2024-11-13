
using static RentalCheckIn.DTOs.CustomResponses;

namespace RentalCheckIn.Services.Core;
public class AccountService : IAccountService
{
    private readonly IHostRepository hostRepository;
    private readonly IConfiguration configuration;
    private readonly IEmailService emailService;
    private readonly IRefreshTokenRepository refreshTokenRepository;

    public AccountService(IHostRepository hostRepository, IConfiguration configuration, IEmailService emailService, IRefreshTokenRepository refreshTokenRepository)
    {
        this.hostRepository = hostRepository;
        this.configuration = configuration;
        this.emailService = emailService;
        this.refreshTokenRepository = refreshTokenRepository;
    }
    public async Task<OperationResult<LHost>> LoginAsync(HostLoginDto hostLoginDto)
    {
        try
        {

            var lHost = await hostRepository.GetLHostByEmailAsync(hostLoginDto.Email);
            if (lHost == null)
            {
                return new OperationResult<LHost>
                {
                    IsSuccess = false,
                    Message = "Create an account to proceed."
                };
            }

            // Check if user is blocked
            if (lHost.IsBlockedSince.HasValue)
            {
                // Lift the block after 15 minutes
                TimeSpan blockDuration = TimeSpan.FromMinutes(15);
                var remainingTime = blockDuration - (DateTime.UtcNow - lHost.IsBlockedSince.Value);

                if (remainingTime > TimeSpan.Zero)
                {
                    // User is still blocked
                    string formattedRemainingTime = $"{remainingTime.Minutes} minutes and {remainingTime.Seconds} seconds";

                    return new OperationResult<LHost>
                    {
                        IsSuccess = false,
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

            // Check if email is confirmed
            if (!lHost.EmailConfirmed)
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
                return new OperationResult<LHost>
                {
                    IsSuccess = false,
                    Message = "Please verify your email address. A verification link was sent to your email."
                };
            }

            if (!VerifyPassword(hostLoginDto.Password, lHost.PasswordHash))
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

                return new OperationResult<LHost>
                {
                    IsSuccess = false,
                    Message = "Invalid login credentials"
                };

            }

            await hostRepository.UpdateLHostPartialAsync(lHost, host =>
            {
                host.LastLogin = DateTime.UtcNow;
                // Reset login attempt to zero for a successful login
                host.LoginAttempts = 0;
            });

            return new OperationResult<LHost>
            {
                IsSuccess = true,
                Data = lHost,
            };
        }
        catch (Exception ex)
        {
            // Implement logging to store the actual errors

            return new OperationResult<LHost>
            {
                IsSuccess = false,
                Message = "An unexpected error has occurred."
            };
        }
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task<OperationResult<LHost>> RegisterAsync(HostSignUpDto hostSignUpDto)
    {
        try
        {
            var existingHost = await hostRepository.GetLHostByEmailAsync(hostSignUpDto.Email);
            if (existingHost != null)
            {
                return new OperationResult<LHost>
                {
                    IsSuccess = false,
                    Message = "User already exists"
                };
            }

            var totpService = new TotpService();
            var totpSecret = totpService.GenerateSecret();

            var lHost = new LHost
            {
                FirstName = hostSignUpDto.FirstName,
                LastName = hostSignUpDto.LastName,
                PasswordHash = HashPassword(hostSignUpDto.Password),
                TotpSecret = totpSecret,
                Username = hostSignUpDto.Email,
                MailAddress = hostSignUpDto.Email,
                EmailVerificationToken = GenerateRandomToken(),
                EmailVTokenExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await hostRepository.AddLHostAsync(lHost);

            // Return the TOTP secret to the caller to display to the user
            lHost.TotpSecret = totpSecret;
            var encodedToken = HttpUtility.UrlEncode(lHost.EmailVerificationToken);
            // Send the new verification email
            var verificationLink = $"{configuration["ApplicationSettings:AppUrl"]}/email-confirmation?emailToken={encodedToken}";
            await emailService.SendEmailAsync(lHost.MailAddress, "Confirm your email", $"Please confirm your email by clicking <a href=\"{verificationLink}\">here</a>.");

            return new OperationResult<LHost>
            {
                IsSuccess = true,
                Data = lHost,
            };
        }
        catch (Exception ex) 
        {
            // Implement logging to store the actual errors

            return new OperationResult<LHost>
            {
                IsSuccess = false,
                Message = "An unexpected error has occurred."
            };

        }
    }

    public async Task<EmailVerificationResponse> VerifyEmailTokenAsync(string token)
    {
        try
        {
            // Retrieve user by token from the repository
            var lHost = await hostRepository.GetLHostByEmailVerificationTokenAsync(token);

            // Check if user or token is invalid
            // Counter-check the purpose of the logic
            if (lHost == null)
            {
                return new EmailVerificationResponse
                {
                    IsSuccess = false,
                    // Invalid request
                    Message = "Invalid request"
                };
            }

            // Check if the token has expired
            if (lHost.EmailVTokenExpiresAt < DateTime.UtcNow)
            {
                return new EmailVerificationResponse
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

            return new EmailVerificationResponse
            {
                IsSuccess = true,
                Message = "Email confirmed successfully."
            };
        }
        catch(Exception ex)
        {
            // Implement logging to store the actual errors

            return new EmailVerificationResponse
            {
                IsSuccess = false,
                Message = "An unexpected error has occurred."
            };
        }
    }

    public async Task<OperationResult<RefreshToken>> GetRefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Implement response Dto to handle edge cases
            var tokenEntity = await refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            // Check if the token was found
            if (tokenEntity == null)
            {
                return new OperationResult<RefreshToken>
                {
                    IsSuccess = false,
                    Message = "Refresh token not found."
                };
            }

            return new OperationResult<RefreshToken>
            {
                IsSuccess = true,
                Data = tokenEntity
            };
        }
        catch (Exception ex) 
        {
            // Implement logging to store the actual errors

            return new OperationResult<RefreshToken>
            {
                IsSuccess = false,
                Message = "An unexpected error has occurred."
            };
        }
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public string GenerateRandomToken()
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

    public async Task<OperationResult<LHost>> GetLHostByIdAsync(uint id)
    {
        try
        {
            var lHost = await hostRepository.GetLHostByIdAsync(id);
            if (lHost == null)
            {
                return new OperationResult<LHost>
                {
                    IsSuccess = false,
                    Message = "Host not found."
                };
            }
            return new OperationResult<LHost>
            {
                IsSuccess = true,
                Data = lHost
            };
        }
        catch(Exception ex)
        {
            // Implement logging to store the actual errors
            return new OperationResult<LHost>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred. Please try again later."
            };
        }
    }

    public async Task<LHost> GetLHostByEmailAsync(string email)
    {
        try
        {
            return await hostRepository.GetLHostByEmailAsync(email);
        }
        catch (Exception ex) 
        {
            // log error
            return new LHost();
        }
    }

    public async Task<OperationResult> ForgotPasswordAsync(LHost lHost)
    {
        try
        {
            string passResetToken = GenerateRandomToken();

            bool result = await hostRepository.UpdateLHostPartialAsync(lHost, host =>
            {
                host.PasswordResetToken = passResetToken;
                host.ResetTokenExpires = DateTime.UtcNow.AddDays(2);
            });

            if (!result)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "An error occurred."
                };
            }

            var encodedToken = HttpUtility.UrlEncode(passResetToken);
            // Send the new verification email
            var resetLink = $"{configuration["ApplicationSettings:AppUrl"]}/reset-password?resetToken={encodedToken}";
            await emailService.SendEmailAsync(lHost.MailAddress, "Confirm your email", $"Please click <a href=\"{resetLink}\">here</a> to reset your password.");

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Password reset request successful, check your email to reset your password."
            };
        }
        catch (Exception ex)
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = "An unexpected error occurred. Please try again later."
            };
        }
    }

    public async Task<OperationResult<string>> ResetPasswordAsync(PasswordResetRequest request)
    {
        try
        {
            var lHost = await hostRepository.GetLHostByPasswordResetTokenAsync(request.ResetToken);
            if (lHost == null)
            {
                return new OperationResult<string>
                {
                    IsSuccess = false,
                    Message = "Invalid reset link"
                };
            }


            // Determine what to do with the bool return type
            bool result = await hostRepository.UpdateLHostPartialAsync(lHost, host =>
             {
                 host.PasswordHash = HashPassword(request.NewPassword);
                 host.PasswordResetToken = null;
                 host.ResetTokenExpires = null;
             });

            if (!result)
            {
                return new OperationResult<string>
                {
                    IsSuccess = false,
                    Message = "An error occurred while resetting your password."
                };
            }

            return new OperationResult<string>
            {
                IsSuccess = true,
                Message = "Your password has been changed successfully.",
                Data = lHost.MailAddress
            };
        }
        catch (Exception ex)
        { 
            return new OperationResult<string>
            {
                IsSuccess = false,
                Message = "An unexpected error has occurred. Please try again later."
            };
        }
    }
}
