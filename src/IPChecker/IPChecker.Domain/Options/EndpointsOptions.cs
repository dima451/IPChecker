namespace IPChecker.Domain.Options;

public class EndpointsOptions
{
    public const string Name = "Endpoints";
    
    /// <summary>
    /// Cache API endpoint
    /// </summary>
    public string CacheApi { get; set; } = "https://localhost:5001";
    
    /// <summary>
    /// Lookup API endpoint
    /// </summary>
    public string LookupApi { get; set; } = "https://localhost:5002";
    
    /// <summary>
    /// Batch API endpoint
    /// </summary>
    public string BatchApi { get; set; } = "https://localhost:5003";
}