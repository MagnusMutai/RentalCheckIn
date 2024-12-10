using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace RentalCheckIn.Components.Layout;

public class CultureSelectorBase : ComponentBase
{
    private Dictionary<uint, CultureFlagDTO> flagGroups;

    protected string englishFlag = string.Empty;
    protected string dutchFlag = string.Empty;
    protected string frenchFlag = string.Empty;
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;
    [Inject]
    private ILogger<CultureSelectorBase> Logger { get; set; } = default!;
    [Inject]
    private ILocalizationUIService LocalizationUIService { get; set; } = default!;
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
                Logger.LogError(ex, "An unexpected error occurred while trying to set the culture in Culture selector component.");
            }
        }
    }

    protected MarkupString CurrentFlag
    {
        get
        {
            return Culture.Name switch
            {
                "en-EN" => new MarkupString(englishFlag),
                "fr-FR" => new MarkupString(frenchFlag),
                "nl-NL" => new MarkupString(dutchFlag),
                _ => new MarkupString(englishFlag) // fallback
            };
        }
    }


    protected override async Task OnInitializedAsync()
    {
        try
        {
            var allFlags = await LocalizationUIService.GetAllLanguageFlagsAsync();
            if (allFlags is null || !allFlags.Any())
            {
                Logger.LogWarning("No language flags were retrieved.");
                flagGroups = new Dictionary<uint, CultureFlagDTO>();
            }
            else
            {
                flagGroups = allFlags.ToDictionary(cf => cf.Id);
            }

            AssignFlags();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load language flags in CultureSelectorBase.");
            flagGroups = new Dictionary<uint, CultureFlagDTO>();
        }
    }



    private void AssignFlags()
    {
        // Assign the SVG strings based on IDs
        englishFlag = GetCultureFlagSvgById(1);
        dutchFlag = GetCultureFlagSvgById(2);
        frenchFlag = GetCultureFlagSvgById(3);
    }

    // Get the SVG string by ID
    private string GetCultureFlagSvgById(uint id)
    {
        return flagGroups.TryGetValue(id, out var cultureFlag) ? cultureFlag.FlagSvg : string.Empty;
    }

    protected void SetCulture(string cultureName)
    {
        try
        {
            var cultureInfo = new CultureInfo(cultureName);
            Culture = cultureInfo;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to set culture: {cultureName}", cultureName);
        }
    }
}
