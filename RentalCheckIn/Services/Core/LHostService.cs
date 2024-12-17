
using System.Net.Mail;

namespace RentalCheckIn.Services.Core;
public class LHostService : ILHostService
{
    private readonly ILHostRepository lHostRepository;
    private readonly ILogger<LHostService> logger;

    public LHostService(ILHostRepository lHostRepository, ILogger<LHostService> logger)
    {
        this.lHostRepository = lHostRepository;
        this.logger = logger;
    }

    // Result pattern
    public async Task<LHost> GetLHostByEmailAsync(string mailAddress)
    {
        try
        {
           var lHost = await lHostRepository.GetLHostByEmailAsync (mailAddress);
           
            if(lHost == null)
            {
                return new LHost();
            }

            return lHost;
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error occurred in LHostService while trying to get LHost by email.");
            return new LHost();
        }
    }

    public async Task<LHost> GetLHostByIdAsync(uint id)
    {
        try
        {
            var lHost = await lHostRepository.GetLHostByIdAsync(id);

            if (lHost == null)
            {
                return new LHost();
            }

            return lHost;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in LHostService while trying to get LHost by email.");
            return new LHost();
        }
    }
}
