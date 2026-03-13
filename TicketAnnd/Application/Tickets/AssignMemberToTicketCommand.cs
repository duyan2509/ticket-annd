using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Application.Tickets;

public record AssignMemberToTicketCommand(Guid CompanyId, Guid TicketId, Guid MemberUserId, Guid ActorId, string? ActorEmail) : IRequest<Guid>;

public class AssignMemberToTicketCommandHandler : IRequestHandler<AssignMemberToTicketCommand, Guid>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;

    public AssignMemberToTicketCommandHandler(ITeamRepository teamRepository, ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IUserCompanyRoleRepository userCompanyRoleRepository)
    {
        _teamRepository = teamRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _userCompanyRoleRepository = userCompanyRoleRepository;
    }

    public async Task<Guid> Handle(AssignMemberToTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTrackingByIdCompanyIdAsync(request.TicketId, request.CompanyId, cancellationToken);
        if (ticket is null)
            throw new BadRequestException("Ticket not found in company");

        if (ticket.Status == TicketStatuses.Resolved)
            throw new BadRequestException("Cannot perform action because ticket is resolved");

        if (!ticket.TeamId.HasValue)
            throw new BadRequestException("Ticket has no team assigned");

        var member = await _teamRepository.GetMemberByUserIdAndTeamIdAsync(request.MemberUserId, ticket.TeamId.Value, cancellationToken);
        if (member is null)
            throw new BadRequestException("User is not a member of the ticket's team");

        var isCompanyAdmin = false;
        var role = await _userCompanyRoleRepository.GetByUserIdAndCompanyIdAsync(request.ActorId, request.CompanyId, cancellationToken);
        if (role != null && role.Role == AppRoles.CompanyAdmin)
            isCompanyAdmin = true;

        var isLeader = false;
        var actorMember = await _teamRepository.GetMemberByUserIdAndTeamIdAsync(request.ActorId, ticket.TeamId.Value, cancellationToken);
        if (actorMember != null)
        {
            var team = await _teamRepository.GetByIdAsync(ticket.TeamId.Value, cancellationToken);
            if (team != null && team.LeaderId == actorMember.Id)
                isLeader = true;
        }

        if (!(isCompanyAdmin || isLeader))
            throw new ForbiddenException("Not allowed to assign member to this ticket");

        string? actorName = request.ActorEmail;
        string? assigneeName = member.User?.Email;

        var note = assigneeName is not null ? $"Assigned to member {assigneeName}" : $"Assigned to member {request.MemberUserId}";
        ticket.AssignMember(request.MemberUserId, request.ActorId, actorName: actorName, targetName: assigneeName, note: note);

        await _ticketRepository.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
