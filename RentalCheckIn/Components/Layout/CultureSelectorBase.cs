using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace RentalCheckIn.Components.Layout;

public class CultureSelectorBase : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private ILogger<CultureSelectorBase> Logger { get; set; }

    protected override void OnInitialized()
    {
        Culture = CultureInfo.CurrentCulture;
    }

    // Implement user friendly feedback incase of an error
    protected CultureInfo Culture
    {
        get
        {
            return CultureInfo.CurrentCulture;
        }
        set
        {
            try
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var uri = new Uri(Navigation.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                    var cultureEscaped = Uri.EscapeDataString(value.Name);
                    var uriEscaped = Uri.EscapeDataString(uri);

                    Navigation.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error setting the culture.");
            }
        }
    }
}
