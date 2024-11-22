namespace RentalCheckIn.Services.UI;
public class ReservationService : IReservationService
{
    private readonly HttpClient httpClient;

    public ReservationService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("api/reservation/AllTableReservations");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new List<ReservationDTO>();
                }
                return await response.Content.ReadFromJsonAsync<IEnumerable<ReservationDTO>>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"{message}");
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
   
    public async Task<CheckInReservationDTO> GetCheckInReservationByIdAsync(uint reservationId)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/reservation/CheckInFormReservation/{reservationId}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new CheckInReservationDTO();
                }
                return await response.Content.ReadFromJsonAsync<CheckInReservationDTO>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"{message}");
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async Task<bool> UpdateCheckInFormReservationAsync(CheckInReservationDTO checkInModel)
    {
        try
        {
            var updateModel = new CheckInReservationUpdateDTO
            {
                Id = checkInModel.Id,
                PassportNr = checkInModel.PassportNr,
                MailAddress = checkInModel.MailAddress,
                Mobile = checkInModel.Mobile,
                ApartmentFee = checkInModel.ApartmentFee,
                SecurityDeposit = checkInModel.SecurityDeposit,
                TotalPrice = checkInModel.TotalPrice,
                KwhAtCheckIn = checkInModel.KwhAtCheckIn,
                AgreeEnergyConsumption = checkInModel.AgreeEnergyConsumption,
                ReceivedKeys = checkInModel.ReceivedKeys,
                AgreeTerms = checkInModel.AgreeTerms,
                SignatureDataUrl = checkInModel.SignatureDataUrl,
                Place = checkInModel.Place
            };

            var response = await httpClient.PutAsJsonAsync($"api/Reservation/{updateModel.Id}", updateModel);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        { 
            // Implement logging
            return false;
        }
    }

    public async Task<IEnumerable<Setting>> GetSettingsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("api/reservation/settings");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new List<Setting>();
                }
                return await response.Content.ReadFromJsonAsync<IEnumerable<Setting>>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"{message}");
            }
        }
        catch (Exception ex)
        {
            throw;
        }

    }

}
