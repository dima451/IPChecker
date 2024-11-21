namespace IPChecker.Domain.Options;

public class EndpointsOptions
{
    public const string Name = "Endpoints";
    
    /// <summary>
    /// Cache API endpoint
    /// </summary>
    public string CacheApi { get; set; }
    
    /// <summary>
    /// Lookup API endpoint
    /// </summary>
    public string LookupApi { get; set; }
    
    /// <summary>
    /// Batch API endpoint
    /// </summary>
    public string BatchApi { get; set; }
}