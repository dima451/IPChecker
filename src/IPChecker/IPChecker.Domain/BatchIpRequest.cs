namespace IPChecker.Domain;

/// <summary>
/// Batch IP request 
/// </summary>
/// <param name="IpAddresses"></param>
/// <example>192.168.1.1,192.168.1.2,192.168.1.3</example>
public record BatchIpRequest(IEnumerable<string> IpAddresses);