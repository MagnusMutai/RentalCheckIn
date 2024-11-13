namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly RefreshTokenService refreshTokenService;

    public AuthController(IAccountService accountService, RefreshTokenService refreshTokenService)
    {
        this.accountService = accountService;
        this.refreshTokenService = refreshTokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<OperationResult<LHost>>> Login(HostLoginDto request)
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
    public async Task<ActionResult<OperationResult<LHost>>> Register(HostSignUpDto request)
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

}
