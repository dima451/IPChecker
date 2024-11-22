using IPChecker.Domain;
using IPChecker.Domain.Exceptions;
using IPChecker.Domain.Options;
using IpStack.Models;
using IpStack.Services;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using ZiggyCreatures.Caching.Fusion;


namespace IPChecker.CacheApi.Services;

/// <inheritdoc />
public class IpRequestService : IIpRequestService
{
    private readonly IIpStackService _ipStackService;
    private readonly ILogger<IpRequestService> _logger;
    private readonly IFusionCache _cache;
    private readonly CacheOptions _cacheOptions;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ipStackService"></param>
    /// <param name="cache"></param>
    /// <param name="cacheOptions"></param>
    /// <param name="host"></param>
    /// <param name="logger"></param>
    public IpRequestService(IIpStackService ipStackService,
                            IFusionCache cache,
                            IOptions<CacheOptions> cacheOptions,
                            ILogger<IpRequestService> logger)
    {
        _ipStackService = ipStackService;
        
        _cacheOptions = cacheOptions.Value;
        _cache = cache;
        
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task<IpAddressDetails> GetIpInfoAsync(string ipRequest)
    {
        var cacheKey = $"IpRequest:{ipRequest}";
        
        var cachedIpRequest = await _cache.GetOrSetAsync(cacheKey, _ => 
            GetIpDetailsAsync(ipRequest),
            _cacheOptions.IpCacheDuration);
        
       
        if(cachedIpRequest == null)
        {
            _logger.LogError("Failed to get IP details for {IpAddress}", ipRequest);
            throw new IPServiceNotAvailableException("Failed to get IP details");
        }
       
        return cachedIpRequest;
    }

    public async Task<IEnumerable<IpAddressDetails>?> GetBatchIpInfoAsync(IEnumerable<string> batchIpRequest)
    {
        var result = new List<IpAddressDetails>();
        
        foreach (var ipAddress in batchIpRequest)
        {
            result.Add(await GetIpInfoAsync(ipAddress));
        }

        return result;
    }

    private async Task<IpAddressDetails?> GetIpDetailsAsync(string ipAddress)
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(delay);
            
        var ipDetails = await retryPolicy.ExecuteAndCaptureAsync(async () => await _ipStackService.GetIpAddressDetailsAsync(ipAddress: ipAddress));
        
        if (ipDetails.Outcome == OutcomeType.Failure)
        {
            _logger.LogError("Failed to get IP details for {IpAddress}", ipAddress);
            return null;
        }
        
        return ipDetails.Result;
    }
}