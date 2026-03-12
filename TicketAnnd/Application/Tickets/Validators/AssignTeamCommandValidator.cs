using System;
using FluentValidation;

namespace TicketAnnd.Application.Tickets;

public class AssignTeamCommandValidator : AbstractValidator<AssignTeamCommand>
{
    public AssignTeamCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required");
        RuleFor(x => x.TicketId).NotEmpty().WithMessage("TicketId is required");
        RuleFor(x => x.TeamId).NotEmpty().WithMessage("TeamId is required");
        RuleFor(x => x.ActorId).NotEmpty().WithMessage("ActorId is required");
    }
}
