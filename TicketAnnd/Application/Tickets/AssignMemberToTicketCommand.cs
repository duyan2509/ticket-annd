using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;

namespace TicketAnnd.Application.Tickets;

public record AssignMemberToTicketCommand(Guid CompanyId, Guid TicketId, Guid MemberUserId, Guid ActorId) : IRequest<Guid>;

public class AssignMemberToTicketCommandHandler : IRequestHandler<AssignMemberToTicketCommand, Guid>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignMemberToTicketCommandHandler(ITeamRepository teamRepository, ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AssignMemberToTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTrackingByIdCompanyIdAsync(request.TicketId, request.CompanyId, cancellationToken);
        if (ticket is null)
            throw new BadRequestException("Ticket not found in company");

        if (!ticket.TeamId.HasValue)
            throw new BadRequestException("Ticket has no team assigned");

        var member = await _teamRepository.GetMemberByUserIdAndTeamIdAsync(request.MemberUserId, ticket.TeamId.Value, cancellationToken);
        if (member is null)
            throw new BadRequestException("User is not a member of the ticket's team");

        ticket.AssignMember(request.MemberUserId, request.ActorId, note: $"Assigned to member {request.MemberUserId}");

        await _ticketRepository.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
