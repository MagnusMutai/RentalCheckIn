using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalCheckIn.BusinessServices;
using static RentalCheckIn.Responses.CustomResponses;

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
    }
}
