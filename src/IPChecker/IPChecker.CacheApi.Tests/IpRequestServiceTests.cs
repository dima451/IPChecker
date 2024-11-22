using IPChecker.CacheApi.Services;
using IPChecker.Domain.Exceptions;
using IPChecker.Domain.Options;
using IpStack.Models;
using IpStack.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ZiggyCreatures.Caching.Fusion;

namespace IPChecker.CacheApi.Tests;

public class IpRequestServiceTests
{
    [Fact]
    public async Task GetIpInfoAsync_ReturnsIpAddressDetails_WhenIpIsValid()
    {
        var ipStackServiceMock = new Mock<IIpStackService>();
        var cacheMock = new Mock<IFusionCache>();
        var optionsMock = new Mock<IOptions<CacheOptions>>();
        var loggerMock = new Mock<ILogger<IpRequestService>>();

        var ipAddress = "127.0.0.1";
        var ipAddressDetails = new IpAddressDetails
        {
            /* set properties */
        };

        optionsMock.Setup(o => o.Value).Returns(new CacheOptions { IpCacheDuration = TimeSpan.FromMinutes(5) });

        var service = new IpRequestService(ipStackServiceMock.Object, cacheMock.Object, optionsMock.Object,
            loggerMock.Object);

        var result = await service.GetIpInfoAsync(ipAddress);

        Assert.NotNull(result);
        Assert.Equal(ipAddressDetails, result);
    }

    [Fact]
    public async Task GetIpInfoAsync_ThrowsException_WhenIpDetailsNotFound()
    {
        var ipStackServiceMock = new Mock<IIpStackService>();
        var cacheMock = new Mock<IFusionCache>();
        var optionsMock = new Mock<IOptions<CacheOptions>>();
        var loggerMock = new Mock<ILogger<IpRequestService>>();

        var ipAddress = "127.0.0.1";

        optionsMock.Setup(o => o.Value).Returns(new CacheOptions { IpCacheDuration = TimeSpan.FromMinutes(5) });


        var service = new IpRequestService(ipStackServiceMock.Object, cacheMock.Object, optionsMock.Object,
            loggerMock.Object);

        await Assert.ThrowsAsync<IPServiceNotAvailableException>(() => service.GetIpInfoAsync(ipAddress));
    }

    [Fact]
    public async Task GetBatchIpInfoAsync_ReturnsIpAddressDetailsList_WhenAllIpsAreValid()
    {
        var ipStackServiceMock = new Mock<IIpStackService>();
        var cacheMock = new Mock<IFusionCache>();
        var optionsMock = new Mock<IOptions<CacheOptions>>();
        var loggerMock = new Mock<ILogger<IpRequestService>>();

        var ipAddresses = new List<string> { "127.0.0.1", "192.168.0.1" };
        var ipAddressDetails = new IpAddressDetails
        {
            /* set properties */
        };

        optionsMock.Setup(o => o.Value).Returns(new CacheOptions { IpCacheDuration = TimeSpan.FromMinutes(5) });
    
        var service = new IpRequestService(ipStackServiceMock.Object, cacheMock.Object, optionsMock.Object,
            loggerMock.Object);

        var result = await service.GetBatchIpInfoAsync(ipAddresses);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, item => Assert.Equal(ipAddressDetails, item));
    }

    [Fact]
    public async Task GetBatchIpInfoAsync_ThrowsException_WhenAnyIpDetailsNotFound()
    {
        var ipStackServiceMock = new Mock<IIpStackService>();
        var cacheMock = new Mock<IFusionCache>();
        var optionsMock = new Mock<IOptions<CacheOptions>>();
        var loggerMock = new Mock<ILogger<IpRequestService>>();

        var ipAddresses = new List<string> { "127.0.0.1", "192.168.0.1" };

        optionsMock.Setup(o => o.Value).Returns(new CacheOptions { IpCacheDuration = TimeSpan.FromMinutes(5) });

        var service = new IpRequestService(ipStackServiceMock.Object, cacheMock.Object, optionsMock.Object,
            loggerMock.Object);

        await Assert.ThrowsAsync<IPServiceNotAvailableException>(() => service.GetBatchIpInfoAsync(ipAddresses));
    }
}