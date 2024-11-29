﻿
using System.Globalization;

namespace RentalCheckIn.Services.Core;
public class ReservationBusinessService : IReservationBusinessService
{
    private readonly IReservationRepository reservationRepository;
    private readonly ILanguageRepository languageRepository;
    private readonly ILogger<ReservationBusinessService> logger;

    public ReservationBusinessService(IReservationRepository reservationRepository, ILanguageRepository languageRepository, ILogger<ReservationBusinessService> logger)
    {
        this.reservationRepository = reservationRepository;
        this.languageRepository = languageRepository;
        this.logger = logger;
    }

    public async Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync()
    {
        try
        {
            var culture = CultureInfo.CurrentCulture.Name;
            var currentLanguage = await languageRepository.GetLanguageByCultureAsync(culture)
                                  ?? await languageRepository.GetDefaultLanguageAsync();

            var defaultLanguage = await languageRepository.GetDefaultLanguageAsync();

            if (defaultLanguage == null)
            {
                throw new Exception("Default language not found in the database.");
            }

            uint languageId = currentLanguage.LanguageId;
            uint defaultLanguageId = defaultLanguage.LanguageId;

            return await reservationRepository.GetAllTableReservationsAsync(languageId, defaultLanguageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in ReservationBusinessService while trying to fetch all reservations for the reservation list table.");
            // Return an empty list on error
            return Enumerable.Empty<ReservationDTO>(); 
        }
    }

    public async Task<CheckInReservationDTO> GetCheckInReservationByIdAsync(uint reservationId)
    {
        try
        {
            return await reservationRepository.GetCheckInReservationByIdAsync(reservationId);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error occurred in ReservationBusinessService while trying to fetch Check-In form Reservation by Id.");
            return new CheckInReservationDTO();
        }
    }

    public async Task<OperationResult> UpdateCheckInReservationPartialAsync(CheckInReservationUpdateDTO checkInReservation)
    {
        try
        {
            var reservation = await reservationRepository.GetReservationByIdAsync(checkInReservation.Id);
            if (reservation == null)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "Could not find the reservation."
                };
            }

            await reservationRepository.UpdateCheckInReservationPartialAsync(reservation, res =>
            {
                // Update only the modified properties
                if (checkInReservation.PassportNr != null && checkInReservation.PassportNr != reservation.Quest.PassportNr)
                {
                    reservation.Quest.PassportNr= checkInReservation.PassportNr;
                }

                if (checkInReservation.MailAddress != null && checkInReservation.MailAddress != reservation.Quest.MailAddress)
                {
                    reservation.Quest.MailAddress = checkInReservation.MailAddress;
                }

                if (checkInReservation.Mobile != null && checkInReservation.Mobile != reservation.Quest.Mobile)
                {
                    reservation.Quest.Mobile = checkInReservation.Mobile;
                }

                if (checkInReservation.ApartmentFee != reservation.ApartmentFee)
                {
                    reservation.ApartmentFee = checkInReservation.ApartmentFee;
                }

                if (checkInReservation.SecurityDeposit != reservation.SecurityDeposit)
                {
                    reservation.SecurityDeposit = checkInReservation.SecurityDeposit;
                }
                
                if (checkInReservation.TotalPrice != reservation.TotalPrice)
                {
                    reservation.TotalPrice = checkInReservation.TotalPrice;
                }

                if (checkInReservation.KwhAtCheckIn != reservation.KwhAtCheckIn)
                {
                    reservation.KwhAtCheckIn = checkInReservation.KwhAtCheckIn;
                }

                if (checkInReservation.AgreeEnergyConsumption != null && checkInReservation.AgreeEnergyConsumption != reservation.AgreeEnergyConsumption)
                {
                    reservation.AgreeEnergyConsumption = checkInReservation.AgreeEnergyConsumption;
                }
                
                if (checkInReservation.ReceivedKeys != null && checkInReservation.ReceivedKeys != reservation.ReceivedKeys)
                {
                    reservation.ReceivedKeys = checkInReservation.ReceivedKeys;
                }
                
                if (checkInReservation.AgreeTerms != null && checkInReservation.AgreeTerms != reservation.AgreeTerms)
                {
                    reservation.AgreeTerms = checkInReservation.AgreeTerms;
                }

                if (checkInReservation.SignatureDataUrl != null && checkInReservation.SignatureDataUrl != reservation.SignatureQuest)
                {
                    reservation.SignatureQuest = checkInReservation.SignatureDataUrl;
                }
            });

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Successfully checked-in."
            };

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in ReservationBusinessService while trying to partially update a reservation from the check-In form.");

            return new OperationResult
            {
                IsSuccess = false,
                Message = "An unexpected error has occurred. Please try again later."
            };
        }

    }

    public async Task<IEnumerable<Setting>> GetSettingsAsync()
    {
        try
        {
            return await reservationRepository.GetSettingsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in ReservationBusinessService while trying to retrieve settings.");

            return Enumerable.Empty<Setting>();
        }
    }

}
