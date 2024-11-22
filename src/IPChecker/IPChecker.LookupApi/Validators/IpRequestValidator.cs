using FluentValidation;
using IPChecker.Domain;

namespace IPChecker.LookupApi.Validators;

/// <summary>
/// IPRequestValidator
/// </summary>
public class IpRequestValidator : AbstractValidator<IpRequest>
{
    public IpRequestValidator()
    {
        RuleFor(x => x.IpAddress)
            .NotEmpty()
            .WithMessage("IpAddress is required")
            .Matches(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
            .WithMessage("IpAddress is not valid");
    }
    
}