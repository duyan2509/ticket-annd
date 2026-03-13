using System;
using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;
using MediatR;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Application.Tickets;

public record AssignTeamCommand(Guid CompanyId, Guid TicketId, Guid TeamId, Guid ActorId, string? ActorEmail) : IRequest<Guid>;

public class AsssignTeamCommandHandler : IRequestHandler<AssignTeamCommand, Guid>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AsssignTeamCommandHandler(ITeamRepository teamRepository, ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AssignTeamCommand request, CancellationToken cancellationToken)
    {
        bool validTeam = await _teamRepository.CheckExistInCompanyAsync(request.CompanyId, request.TeamId, cancellationToken);
        if (!validTeam)
            throw new BadRequestException("Team not found in company");
        var ticket = await _ticketRepository.GetTrackingByIdCompanyIdAsync(request.TicketId, request.CompanyId, cancellationToken);
        if (ticket is null)
            throw new BadRequestException("Ticket not found in company");
        if (ticket.Status == TicketStatuses.Resolved)
            throw new BadRequestException("Cannot perform action because ticket is resolved");
        var fromStatus = ticket.Status;
        string? actorName = request.ActorEmail;
        string? teamName = null;
        try { var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken); teamName = team?.Name; } catch { }

        var note = teamName is not null ? $"Assigned to team {teamName}" : $"Assigned to team {request.TeamId}";
        ticket.AssignTeam(request.TeamId, request.ActorId, actorName: actorName, targetName: teamName, note: note);

        await _ticketRepository.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}