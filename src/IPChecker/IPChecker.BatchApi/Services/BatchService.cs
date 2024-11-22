using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Hangfire;
using Hangfire.Server;
using IPChecker.Domain;
using IPChecker.Domain.Options;
using IpStack.Models;
using Microsoft.Extensions.Options;

namespace IPChecker.BatchApi.Services;

///<inheritdoc/>
public class BatchService : IBatchIpService
{
    private readonly ILogger<BatchService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly EndpointsOptions _endpointsOptions;

    public static readonly ConcurrentDictionary<string, string> JobResults = new();
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="endpointsOptions"></param>
    public BatchService(ILogger<BatchService> logger, 
                        IHttpClientFactory httpClientFactory,
                        IOptions<EndpointsOptions> endpointsOptions)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _endpointsOptions = endpointsOptions.Value;
    }
    
    ///<inheritdoc/>
    public Task<Guid?> GetBatchIpInfoAsync(BatchIpRequest ipRequests)
    {
        var batchId = BackgroundJob.Enqueue(()=> RequestIpInfos(ipRequests.IpAddresses.ToList(), null));
        
        return Task.FromResult<Guid?>(new Guid(batchId));
    }

    ///<inheritdoc/>
    public Task<string> GetProgressAsync(Guid batchId)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var jobDetails = monitoringApi.JobDetails(batchId.ToString());

        var state = jobDetails.History.LastOrDefault()?.StateName ?? "Unknown";
        
        return Task.FromResult(state);
    }

    ///<inheritdoc/>
    public Task<IEnumerable<IpAddressDetails>?> GetBatchResultAsync(Guid batchId)
    {
        if (JobResults.TryGetValue(batchId.ToString(), out var result))
        {
            return Task.FromResult(JsonSerializer.Deserialize<IEnumerable<IpAddressDetails>>(result));
        }

        return Task.FromResult<IEnumerable<IpAddressDetails>?>(null);
    }
    
    public async Task RequestIpInfos(IList<string> ipAddresses, PerformContext? context)
    {
        List<IpAddressDetails> ipDetails = new();
        
        var client = _httpClientFactory.CreateClient();
        
        client.BaseAddress = new Uri(_endpointsOptions.CacheApi);
        
        for (int i = 0; i < ipAddresses.Count; i += 10)
        {
            var ipAddresesForRequest = ipAddresses.Skip(i).Take(3).ToList();
        
            var addresesForRequest = ipAddresesForRequest.ToList();
            
            if (!addresesForRequest.Any())
            {
                break;
            }
            
            var response = await client.PostAsync($"/Cache/GetBatchIpInfo", 
                new StringContent(JsonSerializer.Serialize(addresesForRequest), Encoding.UTF8, "application/json"));
            
            var content = await response.Content.ReadAsStringAsync();
            
            ipDetails.AddRange(JsonSerializer.Deserialize<IEnumerable<IpAddressDetails>>(content) ?? Array.Empty<IpAddressDetails>());
        }
        
        JobResults[context.BackgroundJob.Id] = JsonSerializer.Serialize(ipDetails);
    }
   
}