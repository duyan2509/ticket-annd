using FluentValidation;

namespace TicketAnnd.Application.Auth.Validators;

public class SwitchRoleCommandValidator : AbstractValidator<SwitchRoleCommand>
{
    public SwitchRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User is required.");
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company is required.");
    }
}
