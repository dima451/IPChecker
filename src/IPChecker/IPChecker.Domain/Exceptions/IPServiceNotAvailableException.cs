namespace IPChecker.Domain.Exceptions;

/// <summary>
/// Exception thrown when the IP service is not available 
/// </summary>
public class IPServiceNotAvailableException : Exception
{
    public IPServiceNotAvailableException(string failedToGetIpDetails)
    {
       
    }
}