using FluentValidation;

namespace TicketAnnd.Application.Tickets.Validators;

public class GetTicketByCodeQueryValidator : AbstractValidator<GetTicketByCodeQuery>
{
    public GetTicketByCodeQueryValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.");

        RuleFor(x => x.TicketCode)
            .NotEmpty().WithMessage("TicketCode is required.")
            .MaximumLength(128).WithMessage("TicketCode is too long.");
    }
}
