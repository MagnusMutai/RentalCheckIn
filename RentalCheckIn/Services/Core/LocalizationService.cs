
namespace RentalCheckIn.Services.Core;

public class LocalizationService : ILocalizationService
{
    public Task<string> GetApartmentNameAsync(uint apartmentId, string culture)
    {
        throw new NotImplementedException();
    }

    public Task<(string? CheckInTime, string? CheckOutTime)> GetReservationTimesAsync(uint reservationId, string culture)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetStatusLabelAsync(uint statusId, string culture)
    {
        throw new NotImplementedException();
    }
}