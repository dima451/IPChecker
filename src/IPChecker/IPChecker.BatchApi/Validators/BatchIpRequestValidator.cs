using FluentValidation;
using IPChecker.Domain;

namespace IPChecker.BatchApi.Validators;

public class BatchIpRequestValidator : AbstractValidator<BatchIpRequest>
{
    public BatchIpRequestValidator()
    {
        RuleFor(x => x.IpAddresses).NotEmpty().WithMessage("Please provide at least one IP address");
        RuleForEach(x=>x.IpAddresses).Matches(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
            .WithMessage("Invalid IP address format");
    }
}