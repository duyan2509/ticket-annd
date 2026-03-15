using System;
using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;
using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Application.Tickets;

public record ResolveTicketCommand(Guid CompanyId, Guid TicketId, Guid ActorId, string? Note, string? ActorEmail) : IRequest<Guid>;

public class ResolveTicketCommandHandler : IRequestHandler<ResolveTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ITeamRepository _teamRepository;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;

    public ResolveTicketCommandHandler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IMediator mediator, ITeamRepository teamRepository, IUserCompanyRoleRepository userCompanyRoleRepository)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _teamRepository = teamRepository;
        _userCompanyRoleRepository = userCompanyRoleRepository;
    }

    public async Task<Guid> Handle(ResolveTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTrackingByIdCompanyIdAsync(request.TicketId, request.CompanyId, cancellationToken);
        if (ticket is null)
            throw new BadRequestException("Ticket not found in company");

        if (ticket.Status == TicketStatuses.Resolved)
            throw new BadRequestException("Cannot resolve ticket because it is already resolved");

        var isCompanyAdmin = false;
        var role = await _userCompanyRoleRepository.GetByUserIdAndCompanyIdAsync(request.ActorId, request.CompanyId, cancellationToken);
        if (role != null && role.Role == AppRoles.CompanyAdmin)
            isCompanyAdmin = true;

        var isRaiser = ticket.RaiserId == request.ActorId;
        var isAssignee = ticket.AssigneeId.HasValue && ticket.AssigneeId.Value == request.ActorId;
        var isLeader = false;
        if (ticket.TeamId.HasValue)
        {
            var member = await _teamRepository.GetMemberByUserIdAndTeamIdAsync(request.ActorId, ticket.TeamId.Value, cancellationToken);
            if (member != null)
            {
                var team = await _teamRepository.GetByIdAsync(ticket.TeamId.Value, cancellationToken);
                if (team != null && team.LeaderId == member.Id)
                    isLeader = true;
            }
        }

        if (!(isCompanyAdmin || isRaiser || isAssignee || isLeader))
            throw new ForbiddenException("Not allowed to resolve this ticket");

        string? actorName = request.ActorEmail;

        ticket.Resolve(request.ActorId, actorName: actorName, note: request.Note);

        
        await _ticketRepository.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
