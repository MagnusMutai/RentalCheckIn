namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly RefreshTokenService refreshTokenService;
    private readonly RefreshTokenRepository refreshTokenRepository;
    private readonly JwtService jwtService;
 

    public AuthController(IAccountService accountService, RefreshTokenService refreshTokenService) 
    {
        this.accountService = accountService;
        this.refreshTokenService = refreshTokenService;
        this.refreshTokenRepository = refreshTokenRepository;
        this.jwtService = jwtService;
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
}
