using FluentValidation;
using FluentValidation.Results;
using IPChecker.Domain;
using IPChecker.LookupApi.Controllers;
using IPChecker.LookupApi.Services;
using IpStack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace IPChecker.LookupApi.Tests;

public class IpLookupControllerTests
{
    [Fact]
    public async Task Get_ReturnsOkWithIpAddressDetails_WhenIpAddressIsValid()
    {
        var loggerMock = new Mock<ILogger<IpLookupsController>>();
        var lookupServiceMock = new Mock<ILookupService>();
        var validatorMock = new Mock<IValidator<IpRequest>>();

        var ipAddress = "127.0.0.1";
        var ipRequest = new IpRequest(ipAddress);
        var ipAddressDetails = new IpAddressDetails
        {
            /* set properties */
        };

        validatorMock.Setup(v => v.Validate(It.IsAny<IpRequest>())).Returns(new ValidationResult());
        lookupServiceMock.Setup(s => s.GetIpAddressDetailsAsync(It.IsAny<IpRequest>())).ReturnsAsync(ipAddressDetails);

        var controller = new IpLookupsController(lookupServiceMock.Object, validatorMock.Object, loggerMock.Object);

        var result = await controller.Get(ipAddress) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(ipAddressDetails, ((dynamic)result.Value).IpAddressDetails);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenIpAddressIsInvalid()
    {
        var loggerMock = new Mock<ILogger<IpLookupsController>>();
        var lookupServiceMock = new Mock<ILookupService>();
        var validatorMock = new Mock<IValidator<IpRequest>>();

        var ipAddress = "invalid_ip";
        var validationResult = new ValidationResult(new List<ValidationFailure>
            { new ValidationFailure("IpAddress", "Invalid IP address") });

        validatorMock.Setup(v => v.Validate(It.IsAny<IpRequest>())).Returns(validationResult);

        var controller = new IpLookupsController(lookupServiceMock.Object, validatorMock.Object, loggerMock.Object);

        var result = await controller.Get(ipAddress) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal(validationResult.Errors, result.Value);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenIpAddressDetailsNotFound()
    {
        var loggerMock = new Mock<ILogger<IpLookupsController>>();
        var lookupServiceMock = new Mock<ILookupService>();
        var validatorMock = new Mock<IValidator<IpRequest>>();

        var ipAddress = "127.0.0.1";
        var ipRequest = new IpRequest(ipAddress);

        validatorMock.Setup(v => v.Validate(It.IsAny<IpRequest>())).Returns(new ValidationResult());
        lookupServiceMock.Setup(s => s.GetIpAddressDetailsAsync(It.IsAny<IpRequest>()))
            .ReturnsAsync((IpAddressDetails)null);

        var controller = new IpLookupsController(lookupServiceMock.Object, validatorMock.Object, loggerMock.Object);

        var result = await controller.Get(ipAddress) as NotFoundResult;

        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }
}