using FluentValidation;
using PickC.Modules.Booking.Application.DTOs;

namespace PickC.Modules.Booking.Application.Validators;

public class BookingSaveValidator : AbstractValidator<BookingSaveDto>
{
    public BookingSaveValidator()
    {
        RuleFor(x => x.BookingNo)
            .NotEmpty().WithMessage("Booking number is required.")
            .MaximumLength(50);

        RuleFor(x => x.CustomerID)
            .NotEmpty().WithMessage("Customer ID is required.")
            .MaximumLength(20);

        RuleFor(x => x.RequiredDate)
            .NotEmpty().WithMessage("Required date is required.");

        RuleFor(x => x.LocationFrom)
            .NotEmpty().WithMessage("Pickup location is required.");

        RuleFor(x => x.LocationTo)
            .NotEmpty().WithMessage("Destination location is required.");

        RuleFor(x => x.CargoDescription)
            .MaximumLength(100);

        RuleFor(x => x.CargoType)
            .MaximumLength(100);

        RuleFor(x => x.PayLoad)
            .MaximumLength(50);

        RuleFor(x => x.ReceiverMobileNo)
            .MaximumLength(15);
    }
}

public class BookingCancelValidator : AbstractValidator<BookingCancelDto>
{
    public BookingCancelValidator()
    {
        RuleFor(x => x.BookingNo)
            .NotEmpty().WithMessage("Booking number is required.");

        RuleFor(x => x.CancelRemarks)
            .MaximumLength(100);
    }
}

public class BookingDriverCancelValidator : AbstractValidator<BookingDriverCancelDto>
{
    public BookingDriverCancelValidator()
    {
        RuleFor(x => x.BookingNo)
            .NotEmpty().WithMessage("Booking number is required.");

        RuleFor(x => x.DriverID)
            .NotEmpty().WithMessage("Driver ID is required.");

        RuleFor(x => x.CancelRemarks)
            .MaximumLength(100);
    }
}

public class BookingConfirmValidator : AbstractValidator<BookingConfirmDto>
{
    public BookingConfirmValidator()
    {
        RuleFor(x => x.BookingNo)
            .NotEmpty().WithMessage("Booking number is required.");

        RuleFor(x => x.DriverID)
            .NotEmpty().WithMessage("Driver ID is required.");

        RuleFor(x => x.VehicleNo)
            .NotEmpty().WithMessage("Vehicle number is required.");
    }
}
