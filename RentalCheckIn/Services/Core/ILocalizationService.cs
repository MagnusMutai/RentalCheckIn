namespace RentalCheckIn.Services.Core;

public interface ILocalizationService
{
    Task<string> GetApartmentNameAsync(uint apartmentId, string culture);
    Task<string> GetStatusLabelAsync(uint statusId, string culture);
    Task<(string? CheckInTime, string? CheckOutTime)> GetReservationTimesAsync(uint reservationId, string culture);
}
