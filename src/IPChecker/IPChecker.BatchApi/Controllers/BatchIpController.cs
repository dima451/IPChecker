using FluentValidation;
using IPChecker.BatchApi.Services;
using IPChecker.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IPChecker.BatchApi.Controllers;

///<inheritdoc/>

[ApiController]
[Route("[controller]/[action]")]
[EnableRateLimiting("fixed")]
public class BatchIpController : ControllerBase
{
    private readonly ILogger<BatchIpController> _logger;
    private readonly IBatchIpService _batchIpService;
    private readonly IValidator<BatchIpRequest> _validator;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="batchIpService"></param>
    /// <param name="validator"></param>
    /// <param name="logger"></param>
    public BatchIpController(IBatchIpService batchIpService, 
                             IValidator<BatchIpRequest> validator, 
                             ILogger<BatchIpController> logger)
    {
        _logger = logger;
        _batchIpService = batchIpService;
        _validator = validator;
    }
    
    [HttpPost(Name = "getBatchIpLookup")]
    public async Task<IActionResult> GetBatchIpLookup([FromBody] BatchIpRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var result = await _batchIpService.GetBatchIpInfoAsync(request);
        
        if(result == null)
        {
            return NotFound();
        }
        
        return Ok(new { RequestId = result });
    }
    
    [HttpGet("{batchId}", Name = "getBatchProgress")]
    
    public async Task<IActionResult> GetProgress(Guid batchId)
    {
        var result = await _batchIpService.GetProgressAsync(batchId);
        
        return Ok(new { Progress = result });
    }
    
    [HttpGet("{batchId}", Name = "getBatchResult")]
    
    public async Task<IActionResult> GetResult(Guid batchId)
    {
        var result = await _batchIpService.GetBatchResultAsync(batchId);
        
        if(result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }
}