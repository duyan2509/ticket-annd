using FluentValidation;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Invitation.Validators;

public class RespondInvitationCommandValidator:AbstractValidator<RespondInvitationCommand>
{

    public RespondInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId)
            .NotEmpty();

        RuleFor(x => x.ResponderUserId)
            .NotEmpty();

        RuleFor(x => x.NewStatus)
            .IsInEnum();
        RuleFor(x => x.NewStatus)
            .Must(x => x == InviationStatuses.Accepted || x == InviationStatuses.Rejected);
    }
}
