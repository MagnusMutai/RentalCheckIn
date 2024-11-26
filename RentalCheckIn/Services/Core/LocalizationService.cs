
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace RentalCheckIn.Services.Core;

public class LocalizationService : ILocalizationService
{
    private readonly ILanguageRepository languageRepository;
    private readonly IApartmentTranslationRepository apartmentTranslationRepository;
    private readonly IStatusTranslationRepository statusTranslationRepository;
    private readonly IReservationTranslationRepository reservationTranslationRepository;
    private readonly IMemoryCache cache;

    public LocalizationService(
        ILanguageRepository languageRepository,
        IApartmentTranslationRepository apartmentTranslationRepository,
        IStatusTranslationRepository statusTranslationRepository,
        IReservationTranslationRepository reservationTranslationRepository,
        IMemoryCache cache)
    {
        this.languageRepository = languageRepository;
        this.apartmentTranslationRepository = apartmentTranslationRepository;
        this.statusTranslationRepository = statusTranslationRepository;
        this.reservationTranslationRepository = reservationTranslationRepository;
        this.cache = cache;
    }

    public async Task<Dictionary<uint, string>> GetApartmentNamesAsync(IEnumerable<uint> apartmentIds, string culture)
    {
        try
        {
            // Prepare cache keys
            var cacheKeys = apartmentIds.Select(id => new { Id = id, CacheKey = $"ApartmentName{id}{culture}" }).ToList();
            var result = new Dictionary<uint, string>();
            var idsToFetch = new List<uint>();

            // Check cache first
            foreach (var item in cacheKeys)
            {
                if (cache.TryGetValue(item.CacheKey, out string apartmentName))
                {
                    result[item.Id] = apartmentName;
                }
                else
                {
                    idsToFetch.Add(item.Id);
                }
            }

            if (idsToFetch.Any())
            {
                var language = await languageRepository.GetLanguageByCultureAsync(culture)
                               ?? await languageRepository.GetDefaultLanguageAsync();

                if (language == null)
                {
                    throw new Exception("Default language not found in the database.");
                }

                var translations = await apartmentTranslationRepository.GetTranslationsAsync(idsToFetch, language.LanguageId);

                foreach (var id in idsToFetch)
                {
                    var translation = translations.FirstOrDefault(t => t.ApartmentId == id);
                    var apartmentName = translation?.ApartmentName ?? "[Apartment Name]";
                    result[id] = apartmentName;

                    // Set cache
                    string cacheKey = $"ApartmentName{id}{culture}";
                    cache.Set(cacheKey, apartmentName, TimeSpan.FromHours(1));
                }
            }

            return result;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }


    public async Task<Dictionary<uint, string>> GetStatusLabelsAsync(IEnumerable<uint> statusIds, string culture)
    {
        // Prepare cache keys
        var cacheKeys = statusIds.Select(id => new { Id = id, CacheKey = $"StatusLabel{id}{culture}" }).ToList();
        var result = new Dictionary<uint, string>();
        var idsToFetch = new List<uint>();

        // Check cache first
        foreach (var item in cacheKeys)
        {
            if (cache.TryGetValue(item.CacheKey, out string statusLabel))
            {
                result[item.Id] = statusLabel;
            }
            else
            {
                idsToFetch.Add(item.Id);
            }
        }

        if (idsToFetch.Any())
        {
            var language = await languageRepository.GetLanguageByCultureAsync(culture)
                           ?? await languageRepository.GetDefaultLanguageAsync();

            if (language == null)
            {
                throw new Exception("Default language not found in the database.");
            }

            var translations = await statusTranslationRepository.GetTranslationsAsync(idsToFetch, language.LanguageId);

            foreach (var id in idsToFetch)
            {
                var translation = translations.FirstOrDefault(t => t.StatusId == id);
                var statusLabel = translation?.StatusLabel ?? "[Status Label]";
                result[id] = statusLabel;

                // Set cache
                string cacheKey = $"StatusLabel{id}{culture}";
                cache.Set(cacheKey, statusLabel, TimeSpan.FromHours(1));
            }
        }

        return result;
    }

    public async Task<(string? CheckInTime, string? CheckOutTime)> GetReservationTimesAsync(uint reservationId)
    {
        var culture = CultureInfo.CurrentCulture.Name;
        return await GetReservationTimesAsync(reservationId, culture);
    }

    private async Task<(string? CheckInTime, string? CheckOutTime)> GetReservationTimesAsync(uint reservationId, string culture)
    {
        string cacheKey = $"ReservationTimes{reservationId}{culture}";
        if (cache.TryGetValue(cacheKey, out (string? CheckInTime, string? CheckOutTime) times))
        {
            return times;
        }

        var language = await languageRepository.GetLanguageByCultureAsync(culture)
                       ?? await languageRepository.GetDefaultLanguageAsync();

        if (language == null)
        {
            throw new Exception("Default language not found in the database.");
        }

        var translation = await reservationTranslationRepository.GetTranslationAsync(reservationId, language.LanguageId);

        times = (translation?.CheckInTime, translation?.CheckOutTime);

        cache.Set(cacheKey, times, TimeSpan.FromHours(1));

        return times;
    }
}