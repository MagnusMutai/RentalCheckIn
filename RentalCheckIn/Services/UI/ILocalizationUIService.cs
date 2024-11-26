namespace RentalCheckIn.Services.UI;
public interface ILocalizationUIService
{
    Task<Dictionary<uint, string>> GetApartmentNamesAsync(IEnumerable<uint> apartmentIds, string culture);
    Task<Dictionary<uint, string>> GetStatusLabelsAsync(IEnumerable<uint> statusIds, string culture);
    Task<(string? CheckInTime, string? CheckOutTime)> GetReservationTimesAsync(uint reservationId);
}
