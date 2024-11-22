using System.Text.Json;
using Hangfire.Server;
using IPChecker.BatchApi.Services;
using IPChecker.Domain;
using IPChecker.Domain.Options;
using IpStack.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace IPChecker.BatchApi.Tests;

public class BatchServiceTests
{
    [Fact]
    public async Task GetBatchIpInfoAsync_ReturnsBatchId_WhenIpRequestsAreValid()
    {
        var loggerMock = new Mock<ILogger<BatchService>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var optionsMock = new Mock<IOptions<EndpointsOptions>>();

        optionsMock.Setup(o => o.Value).Returns(new EndpointsOptions { CacheApi = "http://localhost" });

        var service = new BatchService(loggerMock.Object, httpClientFactoryMock.Object, optionsMock.Object);

        var ipRequests = new BatchIpRequest (new List<string> { "127.0.0.1", "192.168.0.1" } );

        var result = await service.GetBatchIpInfoAsync(ipRequests);

        Assert.NotNull(result);
        Assert.IsType<Guid>(result);
    }

    [Fact]
    public async Task GetProgressAsync_ReturnsState_WhenBatchIdIsValid()
    {
        var loggerMock = new Mock<ILogger<BatchService>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var optionsMock = new Mock<IOptions<EndpointsOptions>>();

        optionsMock.Setup(o => o.Value).Returns(new EndpointsOptions { CacheApi = "http://localhost" });

        var service = new BatchService(loggerMock.Object, httpClientFactoryMock.Object, optionsMock.Object);

        var batchId = Guid.NewGuid();

        var result = await service.GetProgressAsync(batchId);

        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task GetBatchResultAsync_ReturnsIpAddressDetails_WhenBatchIdIsValid()
    {
        var loggerMock = new Mock<ILogger<BatchService>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var optionsMock = new Mock<IOptions<EndpointsOptions>>();

        optionsMock.Setup(o => o.Value).Returns(new EndpointsOptions { CacheApi = "http://localhost" });

        var service = new BatchService(loggerMock.Object, httpClientFactoryMock.Object, optionsMock.Object);

        var batchId = Guid.NewGuid();
        var ipAddressDetails = new List<IpAddressDetails>
        {
            new IpAddressDetails
            {
                /* set properties */
            }
        };

        BatchService.JobResults[batchId.ToString()] = JsonSerializer.Serialize(ipAddressDetails);

        var result = await service.GetBatchResultAsync(batchId);

        Assert.NotNull(result);
        Assert.Equal(ipAddressDetails, result);
    }

    [Fact]
    public async Task RequestIpInfos_AddsIpDetailsToJobResults_WhenIpAddressesAreValid()
    {
        var loggerMock = new Mock<ILogger<BatchService>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var optionsMock = new Mock<IOptions<EndpointsOptions>>();
        var httpClientMock = new Mock<HttpMessageHandler>();

        optionsMock.Setup(o => o.Value).Returns(new EndpointsOptions { CacheApi = "http://localhost" });

        var ipAddresses = new List<string> { "127.0.0.1", "192.168.0.1" };
        var ipAddressDetails = new List<IpAddressDetails>
        {
            new IpAddressDetails
            {
                /* set properties */
            }
        };

        
        var httpClient = new HttpClient(httpClientMock.Object);
        httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var service = new BatchService(loggerMock.Object, httpClientFactoryMock.Object, optionsMock.Object);

        var contextMock = new Mock<PerformContext>();
        contextMock.Setup(c => c.BackgroundJob.Id).Returns(Guid.NewGuid().ToString());

        await service.RequestIpInfos(ipAddresses, contextMock.Object);

        Assert.True(BatchService.JobResults.ContainsKey(contextMock.Object.BackgroundJob.Id));
        Assert.Equal(JsonSerializer.Serialize(ipAddressDetails),
            BatchService.JobResults[contextMock.Object.BackgroundJob.Id]);
    }
}