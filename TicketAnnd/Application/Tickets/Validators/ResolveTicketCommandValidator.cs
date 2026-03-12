using System;
using FluentValidation;

namespace TicketAnnd.Application.Tickets;

public class ResolveTicketCommandValidator : AbstractValidator<ResolveTicketCommand>
{
    public ResolveTicketCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required");
        RuleFor(x => x.TicketId).NotEmpty().WithMessage("TicketId is required");
        RuleFor(x => x.ActorId).NotEmpty().WithMessage("ActorId is required");
        RuleFor(x => x.Note).MaximumLength(2000);
    }
}
