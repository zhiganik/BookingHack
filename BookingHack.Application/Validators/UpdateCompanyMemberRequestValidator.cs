using BookingHack.Application.Contracts.Requests;
using FluentValidation;

namespace BookingHack.Application.Validators;

public class UpdateCompanyMemberRequestValidator : AbstractValidator<UpdateCompanyMemberRequest>
{
    public UpdateCompanyMemberRequestValidator()
    {
        RuleFor(x => x.Role).IsInEnum();
    }
}
