using Fido2NetLib;
using Fido2NetLib.Objects;

namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly RefreshTokenService refreshTokenService;
    private readonly Fido2 fido2;
    private readonly ILogger<AuthController> logger;
    private readonly AppDbContext context;

    public AuthController(IAccountService accountService, RefreshTokenService refreshTokenService, Fido2 fido2, ILogger<AuthController> logger, AppDbContext context)
    {
        this.accountService = accountService;
        this.refreshTokenService = refreshTokenService;
        this.fido2 = fido2;
        this.logger = logger;
        this.context = context;
    }
    // Standard JWT Authentication
    [HttpPost("login")]
    public async Task<ActionResult<OperationResult<LHost>>> Login(HostLoginDTO request)
    {
        try
        {
            var result = await accountService.LoginAsync(request);

            if (result == null)
            {
                NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while trying to Login a user.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<OperationResult<LHost>>> Register(HostSignUpDTO request)
    {
        try
        {
            var result = await accountService.RegisterAsync(request);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while trying to register a user.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }

    // Verify the need of using anonymous
    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<ActionResult<EmailVerificationResponse>> VerifyEmail([FromBody] string token)
    {
        try
        {
            var result = await accountService.VerifyEmailTokenAsync(token);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while trying to verify LHost email.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }

    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {

        if (tokenRequest == null)
        {
            return BadRequest("Invalid request.");
        }

        try
        {
            // Access token and refresh token from the request
            var accessToken = tokenRequest.AccessToken;
            var refreshToken = tokenRequest.RefreshToken;
            var response = await refreshTokenService.ValidateAndRefreshTokensAsync(accessToken, refreshToken);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while trying to refresh tokens.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        try
        {
            var lHost = await accountService.GetLHostByEmailAsync(email);
            // In other endpoints like this pass custom response along with the data.
            if (lHost == null)
            {
                var response = new OperationResult
                {
                    IsSuccess = false,
                    Message = "The provided email is not registered."
                };

                return NotFound(response);
            }

            var result = await accountService.ForgotPasswordAsync(lHost);

            if (result == null)
            {
                var response = new OperationResult
                {
                    IsSuccess = false,
                    Message = "An unexpected error has occurred. Please try again later"
                };
                return BadRequest(response);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while trying to send a password reset request.");

            var response = new OperationResult
            {
                IsSuccess = false,
                Message = "An unexpected error has occurred. Please try again later"
            };
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest request)
    {
        try
        {
            var response = await accountService.ResetPasswordAsync(request);

            if (response == null)
            {
                return BadRequest("We could not reset your password");
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while trying to reset password.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
        }
    }

    // Add TOTP Authentication here

    // Fido2 Face Recognition 
    // Validate and save registration response
    [HttpPost("faceid/register")]
    public async Task<IActionResult> RegisterCredential([FromBody] AuthenticatorAttestationRawResponse response)
    {
        try
        {
            var options = JsonSerializer.Deserialize<CredentialCreateOptions>(
                HttpContext.Session.GetString("fido2.registerOptions"));

            if (options == null)
                return BadRequest("Registration options not found in session");

            IsCredentialIdUniqueToUserAsyncDelegate isCredentialIdUniqueToUser = async (uniqueParams, cancellationToken) =>
            {
                var credentialId = uniqueParams.CredentialId;
                var exists = await context.LHostCredentials
                    .AnyAsync(c => c.CredentialId == Convert.ToBase64String(credentialId), cancellationToken);
                // Return true if it does not exist
                return !exists; 
            };
            // Decode the user ID from byte array to string
            string userIdString = Encoding.UTF8.GetString(options.User.Id);
            if (!uint.TryParse(userIdString, out uint hostId))
            {
                return BadRequest("Invalid HostId format.");
            }

            // Retrieve the Host entity
            var host = await context.LHosts.FindAsync(hostId);

            if (host == null)
                return BadRequest("Host not found.");

            // Set the UserHandle to the Fido2User.Id
            // options.User.Id is in byte[] format
            host.UserHandle = options.User.Id; 
            // Update the Host entity
            context.LHosts.Update(host);
            var result = await fido2.MakeNewCredentialAsync(response, options, isCredentialIdUniqueToUser);
            var newCredential = new LHostCredential
            {
                CredentialId = Convert.ToBase64String(result.Result.CredentialId),
                PublicKey = Convert.ToBase64String(result.Result.PublicKey),
                SignCount = result.Result.Counter,
                HostId = uint.Parse(result.Result.User.Id)
            };
            // Create a repository method instead.
            context.LHostCredentials.Add(newCredential);
            await context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while trying to register LHost faceId credentials.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
        }
    }

    // Generate options for registration
    [HttpGet("faceid/register/options")]
    public async Task<IActionResult> GetRegistrationOptions(uint hostId)
    {
        try
        {
            var user = await context.LHosts
                .Include(h => h.Credentials)
                .FirstOrDefaultAsync(h => h.HostId == hostId);

            if (user == null)
                return NotFound("User not found");

            var options = fido2.RequestNewCredential(
                new Fido2User
                {
                    Id = Encoding.UTF8.GetBytes(user.HostId.ToString()),
                    Name = user.Username,
                    DisplayName = $"{user.FirstName} {user.LastName}"
                },
                user.Credentials.Select(c => new PublicKeyCredentialDescriptor
                {
                    Id = Convert.FromBase64String(c.CredentialId),
                    Type = PublicKeyCredentialType.PublicKey
                }).ToList(),
                new AuthenticatorSelection
                {
                    UserVerification = UserVerificationRequirement.Preferred,
                    RequireResidentKey = false
                },
                AttestationConveyancePreference.None
            );

            HttpContext.Session.SetString("fido2.registerOptions", JsonSerializer.Serialize(options));
            return Ok(options);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while generating options for face Id registration.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
        }
    }

    // Generate options for login
    [HttpGet("faceid/authenticate/options")]
    public async Task<IActionResult> GetAuthenticationOptions(uint hostId)
    {
        try
        {
            var user = await context.LHosts
                .Include(h => h.Credentials)
                .FirstOrDefaultAsync(h => h.HostId == hostId);

            if (user == null || !user.Credentials.Any())
                return NotFound("No credentials registered");

            var options = fido2.GetAssertionOptions(
                user.Credentials.Select(c => new PublicKeyCredentialDescriptor
                {
                    Id = Convert.FromBase64String(c.CredentialId),
                    Type = PublicKeyCredentialType.PublicKey
                }).ToList(),
                UserVerificationRequirement.Preferred
            );

            HttpContext.Session.SetString("fido2.authOptions", JsonSerializer.Serialize(options));
            return Ok(options);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while trying to generate options for face Id loging in.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
        }
    }

    // Validate and process login response
    [HttpPost("faceid/authenticate")]
    public async Task<IActionResult> AuthenticateCredential([FromBody] AuthenticatorAssertionRawResponse response)
    {
        try
        {
            var options = JsonSerializer.Deserialize<AssertionOptions>(
                HttpContext.Session.GetString("fido2.authOptions"));

            if (options == null)
                return BadRequest("Authentication options not found in session");

            var storedCredential = await context.LHostCredentials
                .FirstOrDefaultAsync(c => c.CredentialId == Convert.ToBase64String(response.Id));

            if (storedCredential == null)
                return Unauthorized("Invalid credential");

            IsUserHandleOwnerOfCredentialIdAsync isUserHandleOwnerOfCredentialId = async (userHandleParams, cancellationToken) =>
            {
                var userHandle = userHandleParams.UserHandle;
                var credentialId = userHandleParams.CredentialId;
                // Use a repository class instead.
                var user = await context.LHostCredentials.FirstOrDefaultAsync(c =>
                    c.CredentialId == Convert.ToBase64String(credentialId) &&
                    c.Host.UserHandle.SequenceEqual(userHandle), cancellationToken);

                return user != null;
            };

            var result = await fido2.MakeAssertionAsync(
                response,
                options,
                Convert.FromBase64String(storedCredential.PublicKey),
                (uint)storedCredential.SignCount,
                isUserHandleOwnerOfCredentialId
            );

            // Update the signature counter after successful authentication
            storedCredential.SignCount = result.Counter;
            await context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthController while validating LHost authentication face Id credentials.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
        }
    }
}
