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
    public async Task<ActionResult<AuthenticationResponse>> Login(HostLoginDto request)
    {

        var result = await accountService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponse>> Register(HostSignUpDto request)
    {
        var result = await accountService.RegisterAsync(request);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<ActionResult<EmailVerificationResponse>> VerifyEmail([FromBody] string token)
    {
        var result = await accountService.VerifyEmailTokenAsync(token);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {

        if (tokenRequest == null)
        {
            return BadRequest("Invalid request payload.");
        }

        // Access token and refresh token from the request
        var accessToken = tokenRequest.AccessToken;
        var refreshToken = tokenRequest.RefreshToken;

        await refreshTokenService.ValidateAndRefreshTokensAsync(accessToken, refreshToken);
        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var lHost = await accountService.GetLHostByEmailAsync(email);
        if (lHost == null)
        {
            return BadRequest("Host not found");
        }

        var result = await accountService.ForgotPasswordAsync(lHost);
        return Ok(result);    
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest request)
    {
        var response = await accountService.ResetPasswordAsync(request);
        if (response == null)
        {
            return BadRequest("We could not reset your password");
        }
        return Ok(response);
    }

}
