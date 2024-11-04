using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalCheckIn.Services;
using RentalCheckIn.Services.Core;
using static RentalCheckIn.DTOs.CustomResponses;

namespace RentalCheckIn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AuthController(IAccountService accountService) 
        {
            this.accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResult>> Login(HostLoginDto request)
        {

            var result = await accountService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResult>> Register(HostSignUpDto request)
        {
            var result = await accountService.RegisterAsync(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<ActionResult<EmailVerificationResult>> VerifyEmail([FromBody] string token)
        {
            var result = await accountService.VerifyEmailTokenAsync(token);
            return Ok(result);
        }

        //[HttpPost("refresh-token")]
        //public IActionResult RefreshToken([FromBody] string refreshToken)
        //{
        //    var storedToken = _tokenService.GetRefreshToken(refreshToken);

        //    if (storedToken == null || !storedToken.IsActive)
        //        return Unauthorized("Invalid or expired refresh token");

        //    // Generate a new access token
        //    var newAccessToken = _jwtService.GenerateAccessToken(storedToken.UserId);

        //    // Optionally, generate a new refresh token and revoke the old one
        //    storedToken.IsRevoked = true;
        //    var newRefreshToken = _tokenService.GenerateRefreshToken(storedToken.UserId);

        //    return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken.Token });
        //}
    }
}
