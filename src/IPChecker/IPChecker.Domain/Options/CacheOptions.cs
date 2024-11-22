namespace IPChecker.Domain.Options;

/// <summary>
///   Options for configuring the cache.    
/// </summary>
public class CacheOptions
{
    public const string Name = "Cache";
    
    /// <summary>
    ///  Duration of the cache for IP addresses. 
    /// </summary>
    public TimeSpan IpCacheDuration { get; set; } = TimeSpan.FromMinutes(1);
}