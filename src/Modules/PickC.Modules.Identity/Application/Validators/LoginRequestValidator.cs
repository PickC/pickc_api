using FluentValidation;
using PickC.Modules.Identity.Application.DTOs;

namespace PickC.Modules.Identity.Application.Validators;

public class CustomerLoginRequestValidator : AbstractValidator<CustomerLoginRequest>
{
    public CustomerLoginRequestValidator()
    {
        RuleFor(x => x.MobileNo)
            .NotEmpty().WithMessage("Mobile number is required.")
            .Matches(@"^\d{10}$").WithMessage("Mobile number must be 10 digits.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

public class DriverLoginRequestValidator : AbstractValidator<DriverLoginRequest>
{
    public DriverLoginRequestValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage("Driver ID is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
