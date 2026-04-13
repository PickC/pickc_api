using FluentValidation;
using PickC.Modules.Trip.Application.DTOs;

namespace PickC.Modules.Trip.Application.Validators;

public class TripSaveValidator : AbstractValidator<TripSaveDto>
{
    public TripSaveValidator()
    {
        RuleFor(x => x.TripID)
            .NotEmpty().WithMessage("Trip ID is required.")
            .MaximumLength(50);

        RuleFor(x => x.CustomerMobile)
            .NotEmpty().WithMessage("Customer mobile is required.")
            .MaximumLength(20);

        RuleFor(x => x.DriverID)
            .NotEmpty().WithMessage("Driver ID is required.")
            .MaximumLength(20);

        RuleFor(x => x.VehicleNo)
            .NotEmpty().WithMessage("Vehicle number is required.")
            .MaximumLength(20);

        RuleFor(x => x.TotalWeight)
            .MaximumLength(10);

        RuleFor(x => x.CargoDescription)
            .MaximumLength(100);

        RuleFor(x => x.Remarks)
            .MaximumLength(100);
    }
}

public class TripEndValidator : AbstractValidator<TripEndDto>
{
    public TripEndValidator()
    {
        RuleFor(x => x.TripID)
            .NotEmpty().WithMessage("Trip ID is required.");
    }
}

public class TripMonitorSaveValidator : AbstractValidator<TripMonitorSaveDto>
{
    public TripMonitorSaveValidator()
    {
        RuleFor(x => x.DriverID)
            .NotEmpty().WithMessage("Driver ID is required.");

        RuleFor(x => x.TripID)
            .NotEmpty().WithMessage("Trip ID is required.");

        RuleFor(x => x.VehicleNo)
            .NotEmpty().WithMessage("Vehicle number is required.")
            .MaximumLength(15);
    }
}
