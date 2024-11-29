namespace RentalCheckIn.Services.Core;

public interface ILocalizationService
{
    Task<Dictionary<uint, string>> GetApartmentNamesAsync(IEnumerable<uint> apartmentIds, string culture);
    Task<Dictionary<uint, string>> GetStatusLabelsAsync(IEnumerable<uint> statusIds, string culture);
}
