using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalCheckIn.Services;
using RentalCheckIn.Services.Core;
using static RentalCheckIn.DTOs.CustomResponses;

namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly JwtService jwtService;
    private readonly RefreshTokenService refreshTokenService;

    public AuthController(IAccountService accountService, JwtService jwtService, RefreshTokenService refreshTokenService) 
    {
        this.accountService = accountService;
        this.jwtService = jwtService;
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
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("No refresh token provided.");
        }

        // Step 1: Validate the refresh token by retrieving it from the database
        var refreshTokenEntity = await accountService.GetRefreshTokenAsync(refreshToken);
        if (refreshTokenEntity == null)
        {
            return Unauthorized("You're not authorized");
        }

        var returnedToken = refreshTokenEntity.RefreshToken;

        if (returnedToken == null || !returnedToken.IsActive)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        // Step 2: Retrieve the host associated with this refresh token
        var host = await accountService.GetLHostByIdAsync(returnedToken.HostId);
        if (host == null)
        {
            return Unauthorized("Associated host not found.");
        }

        // Step 3: Generate new tokens
        var newAccessToken = jwtService.GenerateToken(host);
        var newRefreshToken = await refreshTokenService.GenerateRefreshToken(host.HostId);

        // Step 4: Revoke the old refresh token and add the new one
        returnedToken.IsRevoked = true;
        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken.Token,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7), // Set your desired expiration
            HostId = host.HostId
        };
        //await _userService.RevokeAndSaveNewRefreshTokenAsync(refreshTokenEntity, newRefreshTokenEntity);

        // Step 5: Return the new tokens in the response body
        return Ok(new TokenResponse
        {
            NewAccessToken = newAccessToken,
            NewRefreshToken = newRefreshToken.Token
        });
    }

}
