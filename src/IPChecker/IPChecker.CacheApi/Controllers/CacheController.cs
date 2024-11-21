using IPChecker.CacheApi.Services;
using IPChecker.Domain;
using Microsoft.AspNetCore.Mvc;

namespace IPChecker.CacheApi.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<IActionResult> Get([FromQuery] string ipAddress)
    {
        var ipRequest = new IpRequest(ipAddress);
        
        var ipInfo = await _ipRequestService.GetIpInfo(ipRequest);
        
        return Ok(ipInfo);
    }
    
}