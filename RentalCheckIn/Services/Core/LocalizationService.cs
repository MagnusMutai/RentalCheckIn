
using Microsoft.Extensions.Caching.Memory;

namespace RentalCheckIn.Services.Core;

public class LocalizationService : ILocalizationService
{
    private readonly ILanguageRepository languageRepository;
    private readonly IApartmentTranslationRepository apartmentTranslationRepository;
    private readonly IStatusTranslationRepository statusTranslationRepository;
    private readonly IMemoryCache cache;
    private readonly ILogger<LocalizationService> logger;

    public LocalizationService(
        ILanguageRepository languageRepository,
        IApartmentTranslationRepository apartmentTranslationRepository,
        IStatusTranslationRepository statusTranslationRepository,
        IMemoryCache cache,
        ILogger<LocalizationService> logger)
    {
        this.languageRepository = languageRepository;
        this.apartmentTranslationRepository = apartmentTranslationRepository;
        this.statusTranslationRepository = statusTranslationRepository;
        this.cache = cache;
        this.logger = logger;
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

                //if (language == null)
                //{
                //    // Handle gracefully by using Result pattern
                //}

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
            logger.LogError(ex, "An unexpected error occurred in LocalizationService while trying to fetch language-specific apartment names.");
            // Result pattern is an alternative
            return new Dictionary<uint, string>();
        }
    }


    public async Task<Dictionary<uint, string>> GetStatusLabelsAsync(IEnumerable<uint> statusIds, string culture)
    {

        var result = new Dictionary<uint, string>();

        try
        {
            if (statusIds == null || !statusIds.Any())
            {
                return result;
            }

            // Prepare cache keys
            var cacheKeys = statusIds.Select(id => new { Id = id, CacheKey = $"StatusLabel{id}{culture}" }).ToList();
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

        }
        catch (Exception ex) 
        {
            // Log the error
            logger.LogError(ex, "An unexpected error occurred while fetching status labels for culture '{Culture}'. Returning fallback labels.", culture);

            // Return fallback labels for all IDs
            foreach (var id in statusIds ?? Enumerable.Empty<uint>())
            {
                result[id] = "[Status Label]";
            }
        }

        return result;
    }

}