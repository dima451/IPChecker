using System.Threading.Tasks;
using FluentValidation;
using IPChecker.Domain;
using IPChecker.LookupApi.Services;
using IpStack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;

namespace IPChecker.LookupApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IpLookupsController : ControllerBase
{
    private readonly ILogger<IpLookupsController> _logger;
    private readonly ILookupService _lookupService;
    private readonly IValidator<IpRequest> _validator;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lookupService"></param>
    /// <param name="validator"></param>
    /// <param name="logger"></param>
    public IpLookupsController(ILookupService lookupService, 
                               IValidator<IpRequest> validator, 
                               ILogger<IpLookupsController> logger)
    {
        _logger = logger;
        _lookupService = lookupService;
        _validator = validator;
    }
    
    [HttpGet(Name = "GetIpLookup")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> Get([FromQuery] string ipAddress)
    {
        var validationResult = _validator.Validate(new IpRequest(ipAddress));
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var result = await _lookupService.GetIpAddressDetailsAsync(new IpRequest(ipAddress));
        
        if(result == null)
        {
            return NotFound();
        }
        
        return Ok(new { IpAddressDetails = result });
    }
  
}