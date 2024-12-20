namespace RentalCheckIn.Services.Core;
public interface IDocumentService
{
    Task<MemoryStream> FillCheckInFormAsync(CheckInReservationDTO model, string culture);
}
