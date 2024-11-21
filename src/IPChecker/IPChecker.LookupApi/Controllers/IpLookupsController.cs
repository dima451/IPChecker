using System.Threading.Tasks;
using IPChecker.Domain;
using IPChecker.LookupApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IPChecker.LookupApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IpLookupsController : ControllerBase
{
    private readonly ILogger<IpLookupsController> _logger;
    private readonly ILookupService _lookupService;
    
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lookupService"></param>
    /// <param name="logger"></param>
    public IpLookupsController(ILookupService lookupService, 
                               ILogger<IpLookupsController> logger)
    {
        _logger = logger;
        _lookupService = lookupService;
    }
    
    [HttpGet(Name = "GetIpLookup")]
    
    public async Task<IActionResult> Get([FromQuery] string ipAddress)
    {
        var result = await _lookupService.GetIpAddressDetails(new IpRequest(ipAddress));
        
        return Ok(new { IpAddressDetails = result });
    }
  
}