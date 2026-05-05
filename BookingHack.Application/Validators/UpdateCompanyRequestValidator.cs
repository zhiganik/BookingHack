using BookingHack.Application.Contracts.Requests;
using FluentValidation;

namespace BookingHack.Application.Validators;

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TypeOfService).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description is not null);
    }
}
