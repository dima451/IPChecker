using System.Net;
using System.Text.Json;
using IPChecker.Domain;
using IPChecker.Domain.Options;
using IPChecker.LookupApi.Services;
using IpStack.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace IPChecker.LookupApi.Tests;

public class LookupServiceTests
{

    [Fact]
    public async Task GetIpAddressDetailsAsync_ReturnsIpAddressDetails_WhenApiCallIsSuccessful()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LookupService>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var optionsMock = new Mock<IOptions<EndpointsOptions>>();
        var httpClient = new HttpClient();

        var ipAddress = new IpRequest("127.0.0.1");
        var expectedResponse = new IpAddressDetails
        {
            /* set properties */
        };

        optionsMock.Setup(o => o.Value).Returns(new EndpointsOptions { CacheApi = "http://localhost" });

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
        };
        httpClientFactoryMock.Setup(f => f.CreateClient("LookupApi")).Returns(httpClient);

        var service = new LookupService(loggerMock.Object, httpClientFactoryMock.Object, optionsMock.Object);

        // Act
        var result = await service.GetIpAddressDetailsAsync(ipAddress);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task GetIpAddressDetailsAsync_ReturnsNull_WhenApiCallFails()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LookupService>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var optionsMock = new Mock<IOptions<EndpointsOptions>>();
        var httpClientMock = new Mock<HttpMessageHandler>();

        var ipAddress = new IpRequest("127.0.0.1");

        optionsMock.Setup(o => o.Value).Returns(new EndpointsOptions { CacheApi = "http://localhost" });
        
        var httpClient = new HttpClient(httpClientMock.Object);
        httpClientFactoryMock.Setup(f => f.CreateClient("LookupApi")).Returns(httpClient);

        var service = new LookupService(loggerMock.Object, httpClientFactoryMock.Object, optionsMock.Object);

        // Act
        var result = await service.GetIpAddressDetailsAsync(ipAddress);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetIpAddressDetailsAsync_LogsError_WhenApiCallFails()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LookupService>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var optionsMock = new Mock<IOptions<EndpointsOptions>>();
        var httpClientMock = new Mock<HttpMessageHandler>();

        var ipAddress = new IpRequest("127.0.0.1");

        optionsMock.Setup(o => o.Value).Returns(new EndpointsOptions { CacheApi = "http://localhost" });
        
        var httpClient = new HttpClient(httpClientMock.Object);
        httpClientFactoryMock.Setup(f => f.CreateClient("LookupApi")).Returns(httpClient);

        var service = new LookupService(loggerMock.Object, httpClientFactoryMock.Object, optionsMock.Object);

        // Act
        await service.GetIpAddressDetailsAsync(ipAddress);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("Failed to get IP address details from cache API.")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }
}