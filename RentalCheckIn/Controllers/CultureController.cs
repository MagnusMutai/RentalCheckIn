using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace BlazorLocalization.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    private readonly ILogger<CultureController> logger;

    public CultureController(ILogger<CultureController> logger)
    {
        this.logger = logger;
    }
    public IActionResult Set(string culture, string redirectUri)
    {
        // Validate the culture parameter
        if (string.IsNullOrWhiteSpace(culture))
        {
            return BadRequest("Culture is required.");
        }

        // Validate the redirect URI to prevent open redirects (security concern)
        if (string.IsNullOrWhiteSpace(redirectUri) || !Url.IsLocalUrl(redirectUri))
        {
            return BadRequest("Invalid redirect URI.");
        }

        try
        {
            // Set the culture cookie if a valid culture is provided
            var requestCulture = new RequestCulture(culture, culture);
            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

            HttpContext.Response.Cookies.Append(cookieName, cookieValue);
        }
        catch (CultureNotFoundException ex)
        {
            // Handle invalid culture format
            logger.LogError(ex, "Culture was not found in CultureController while trying to set the culture.");
            return BadRequest("Invalid culture format.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in CultureController while trying to set the culture.");
            return StatusCode(500, "An error occurred while setting the culture.");
        }

        // Redirect to the specified URI on success
        return LocalRedirect(redirectUri);
    }
}