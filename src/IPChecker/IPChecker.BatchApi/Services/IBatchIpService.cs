using IPChecker.Domain;
using IpStack.Models;

namespace IPChecker.BatchApi.Services;

/// <summary>
/// Service for batch IP lookup.
/// </summary>
public interface IBatchIpService : IService
{
    /// <summary>
    /// Get information about a batch of IP addresses.
    /// </summary>
    /// <param name="ipRequests"></param>
    /// <returns></returns>
    Task<Guid?> GetBatchIpInfoAsync(BatchIpRequest ipRequests);
    
    /// <summary>
    /// Get the progress of a batch lookup.
    /// </summary>
    /// <param name="batchId"></param>
    /// <returns></returns>
    Task<string> GetProgressAsync(Guid batchId);
    
    /// <summary>
    /// Get the result of a batch lookup.
    /// </summary>
    /// <param name="batchId"></param>
    /// <returns></returns>
    Task<IEnumerable<IpAddressDetails>?> GetBatchResultAsync(Guid batchId);
}