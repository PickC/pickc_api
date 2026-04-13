using FluentValidation;
using PickC.Modules.Master.Application.DTOs;

namespace PickC.Modules.Master.Application.Validators;

public class DriverSaveValidator : AbstractValidator<DriverSaveDto>
{
    public DriverSaveValidator()
    {
        RuleFor(x => x.DriverID)
            .NotEmpty().WithMessage("Driver ID is required.")
            .MaximumLength(50);

        RuleFor(x => x.DriverName)
            .NotEmpty().WithMessage("Driver name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(50);

        RuleFor(x => x.VehicleNo)
            .NotEmpty().WithMessage("Vehicle number is required.")
            .MaximumLength(15);

        RuleFor(x => x.MobileNo)
            .NotEmpty().WithMessage("Mobile number is required.")
            .Matches(@"^\d{10}$").WithMessage("Mobile number must be 10 digits.");

        RuleFor(x => x.LicenseNo)
            .NotEmpty().WithMessage("License number is required.")
            .MaximumLength(50);
    }
}
