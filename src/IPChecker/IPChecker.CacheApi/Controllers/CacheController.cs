using IPChecker.CacheApi.Services;
using IPChecker.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IPChecker.CacheApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[EnableRateLimiting("fixed")]
public class CacheController : ControllerBase
{
    private readonly ILogger<CacheController> _logger;
    private readonly IIpRequestService _ipRequestService;

    public CacheController(ILogger<CacheController> logger, IIpRequestService ipRequestService)
    {
        _logger = logger;
        _ipRequestService = ipRequestService;
    }

    [HttpGet(Name = "GetIpInfo")]
    public async Task<IActionResult> GetIpInfo([FromQuery] string ipAddress)
    {
        var ipInfo = await _ipRequestService.GetIpInfoAsync(ipAddress);
        
        return Ok(ipInfo);
    }
    
    [HttpPost(Name = "GetBatchIpInfo")]
    public async Task<IActionResult> GetBatchIpInfo([FromBody] IEnumerable<string> ipAddresses)
    {
        var idRequestId = await _ipRequestService.GetBatchIpInfoAsync(ipAddresses);
        
        return Ok(idRequestId);
    }
    
}