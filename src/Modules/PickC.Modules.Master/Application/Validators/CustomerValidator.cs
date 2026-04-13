using FluentValidation;
using PickC.Modules.Master.Application.DTOs;

namespace PickC.Modules.Master.Application.Validators;

public class CustomerSaveValidator : AbstractValidator<CustomerSaveDto>
{
    public CustomerSaveValidator()
    {
        RuleFor(x => x.MobileNo)
            .NotEmpty().WithMessage("Mobile number is required.")
            .Matches(@"^\d{10}$").WithMessage("Mobile number must be 10 digits.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(15);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.EmailID)
            .MaximumLength(100)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.EmailID));
    }
}
