using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.EntityFrameworkCore;

namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly RefreshTokenService refreshTokenService;
    private readonly Fido2 fido2;
    private readonly AppDbContext context;

    public AuthController(IAccountService accountService, RefreshTokenService refreshTokenService, Fido2 fido2, AppDbContext context)
    {
        this.accountService = accountService;
        this.refreshTokenService = refreshTokenService;
        this.fido2 = fido2;
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
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }

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
            if (lHost == null)
            {
                return NotFound();
            }

            var result = await accountService.ForgotPasswordAsync(lHost);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    // Add TOTP Authentication here

    // Fido2 Face Recognition 

    // Implement error handling
    /// <summary>
    /// Validate and save registration response (Step 2: Register)
    /// </summary>
    [HttpPost("faceid/register")]
    public async Task<IActionResult> RegisterCredential([FromBody] AuthenticatorAttestationRawResponse response)
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

            return !exists; // Return true if it does not exist
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
        host.UserHandle = options.User.Id; // options.User.Id is byte[]

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

        context.LHostCredentials.Add(newCredential);
        await context.SaveChangesAsync();

        return Ok();
    }

    // Implement error handling
    /// <summary>
    /// Generate options for registration (Step 1: Register)
    /// </summary>
    [HttpGet("faceid/register/options")]
    public async Task<IActionResult> GetRegistrationOptions(uint hostId)
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

    // Implement error handling
    /// <summary>
    /// Generate options for login (Step 1: Login)
    /// </summary>
    [HttpGet("faceid/authenticate/options")]
    public async Task<IActionResult> GetAuthenticationOptions(uint hostId)
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
    // Implement error handling
    /// <summary>
    /// Validate and process login response (Step 2: Login)
    /// </summary>
    [HttpPost("faceid/authenticate")]
    public async Task<IActionResult> AuthenticateCredential([FromBody] AuthenticatorAssertionRawResponse response)
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
}
