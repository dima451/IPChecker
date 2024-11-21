using System.Net;
using IPChecker.Domain;
using IPChecker.Domain.Exceptions;
using IPChecker.Domain.Options;
using IpStack.Models;
using IpStack.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    public async Task<IpAddressDetails> GetIpInfo(IpRequest ipRequest)
    {
        var cacheKey = $"IpRequest:{ipRequest.IpAddress}";
        
        var cachedIpRequest = await _cache.GetOrSetAsync(cacheKey, _ => 
            _ipStackService.GetIpAddressDetailsAsync(ipAddress: ipRequest.IpAddress), 
            TimeSpan.FromMinutes(_cacheOptions.IpCacheDurationMinutes));
        
        if(cachedIpRequest == null)
        {
            _logger.LogError("Failed to get IP details for {IpAddress}", ipRequest.IpAddress);
            throw new IPServiceNotAvailableException("Failed to get IP details");
        }
       
        return cachedIpRequest;
    }
}