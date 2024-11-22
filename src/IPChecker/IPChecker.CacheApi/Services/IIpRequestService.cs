using IPChecker.Domain;
using IpStack.Models;

namespace IPChecker.CacheApi.Services;

/// <summary>
/// IP Request Service interface
/// </summary>
public interface IIpRequestService : IService
{   
    /// <summary>
    /// Get IP info
    /// </summary>
    /// <param name="ipRequest"></param>
    /// <returns></returns>
    Task<IpAddressDetails> GetIpInfoAsync(string ipRequest);
    
    /// <summary>
    /// Get batch IP info
    /// </summary>
    /// <param name="batchIpRequest"></param>
    /// <returns></returns>
    Task<IEnumerable<IpAddressDetails>?> GetBatchIpInfoAsync(IEnumerable<string> batchIpRequest);
}