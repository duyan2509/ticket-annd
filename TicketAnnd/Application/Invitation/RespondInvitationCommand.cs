using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain;

namespace TicketAnnd.Application.Invitation;

public record RespondInvitationCommand(Guid InvitationId, Guid ResponderUserId, InviationStatuses NewStatus) : IRequest<Unit>;

public class RespondInvitationCommandHandler(
    IInvitationRepository invitationRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RespondInvitationCommand, Unit>
{
    public async Task<Unit> Handle(RespondInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);

        if (invitation is null)
            throw new NotFoundException("Invitation not found");

        invitation.Respond(request.ResponderUserId, request.NewStatus);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
