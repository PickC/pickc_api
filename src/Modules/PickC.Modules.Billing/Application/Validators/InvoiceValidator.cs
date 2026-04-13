using FluentValidation;
using PickC.Modules.Billing.Application.DTOs;

namespace PickC.Modules.Billing.Application.Validators;

public class InvoiceSaveValidator : AbstractValidator<InvoiceSaveDto>
{
    public InvoiceSaveValidator()
    {
        RuleFor(x => x.InvoiceNo)
            .NotEmpty().WithMessage("Invoice number is required.")
            .MaximumLength(50);

        RuleFor(x => x.TripID)
            .NotEmpty().WithMessage("Trip ID is required.")
            .MaximumLength(50);

        RuleFor(x => x.TripAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Trip amount cannot be negative.");

        RuleFor(x => x.TaxAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Tax amount cannot be negative.");

        RuleFor(x => x.TipAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Tip amount cannot be negative.");

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative.");

        RuleFor(x => x.BookingNo)
            .MaximumLength(50);
    }
}

public class InvoicePayValidator : AbstractValidator<InvoicePayDto>
{
    public InvoicePayValidator()
    {
        RuleFor(x => x.InvoiceNo)
            .NotEmpty().WithMessage("Invoice number is required.");

        RuleFor(x => x.TripID)
            .NotEmpty().WithMessage("Trip ID is required.");

        RuleFor(x => x.PaidAmount)
            .GreaterThan(0).WithMessage("Paid amount must be greater than zero.");
    }
}
