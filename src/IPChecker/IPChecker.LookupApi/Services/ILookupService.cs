using System.Threading.Tasks;
using IPChecker.Domain;
using IpStack.Models;

namespace IPChecker.LookupApi.Services;

public interface ILookupService : IService
{
    Task<IpAddressDetails> GetIpAddressDetails(IpRequest ipAddress);
}