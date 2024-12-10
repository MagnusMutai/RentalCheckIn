namespace RentalCheckIn.Services.UI;
public interface ILocalizationUIService
{
    Task<List<CultureFlagDTO>> GetAllLanguageFlagsAsync();
    Task<Dictionary<uint, string>> GetApartmentNamesAsync(IEnumerable<uint> apartmentIds, string culture);
    Task<Dictionary<uint, string>> GetStatusLabelsAsync(IEnumerable<uint> statusIds, string culture);
}
