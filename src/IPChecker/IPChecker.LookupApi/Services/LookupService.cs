using IPChecker.Domain;
using IPChecker.Domain.Options;
using IpStack.Models;
using Microsoft.Extensions.Options;

namespace IPChecker.LookupApi.Services;

/// <summary>
/// Lookup service.
/// </summary>
public class LookupService : ILookupService
{
    private readonly ILogger<LookupService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly EndpointsOptions _endpointsOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupService" /> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="endpointsOptions"></param>
    public LookupService(ILogger<LookupService> logger,
                         IHttpClientFactory httpClientFactory,
                         IOptions<EndpointsOptions> endpointsOptions)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _endpointsOptions = endpointsOptions.Value;
    }
    
    /// <summary>
    /// Get IP address details.
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IpAddressDetails> GetIpAddressDetails(IpRequest ipAddress)
    {
        var httpClient = _httpClientFactory.CreateClient("LookupApi");

        httpClient.BaseAddress = new Uri(_endpointsOptions.CacheApi);
        
        await httpClient.GetAsync($"{_endpointsOptions.CacheApi}/cache/GetIpInfo?ipAddress={ipAddress.IpAddress}");
        
        throw new System.NotImplementedException();
    }
}