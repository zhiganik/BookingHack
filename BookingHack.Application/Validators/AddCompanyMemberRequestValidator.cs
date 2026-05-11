using BookingHack.Application.Contracts.Requests;
using FluentValidation;

namespace BookingHack.Application.Validators;

public class AddCompanyMemberRequestValidator : AbstractValidator<AddCompanyMemberRequest>
{
    public AddCompanyMemberRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
    }
}
