using System;
using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain;

namespace TicketAnnd.Application.Tickets;

public record StartTicketCommand(Guid CompanyId, Guid TicketId, Guid ActorId, string? ActorEmail) : IRequest<Guid>;

public class StartTicketCommandHandler : IRequestHandler<StartTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartTicketCommandHandler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(StartTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTrackingByIdCompanyIdAsync(request.TicketId, request.CompanyId, cancellationToken);
        if (ticket is null)
            throw new BadRequestException("Ticket not found in company");

        if (!ticket.AssigneeId.HasValue || ticket.AssigneeId.Value != request.ActorId)
            throw new ForbiddenException("Only assignee can mark start of work");

        if (ticket.FirstResponseAt == DateTime.MinValue)
        {
            ticket.FirstResponseAt = DateTime.UtcNow;
            ticket.AddActionEvent("StartWork", request.ActorId, actorName: request.ActorEmail);
        }

        if (ticket.Status != TicketStatuses.InProgress)
        {
            var from = ticket.Status.ToString();
            ticket.Status = TicketStatuses.InProgress;
            ticket.AddActionEvent("StartWorkSetInProgress", request.ActorId, actorName: request.ActorEmail, fromStatus: from, toStatus: ticket.Status.ToString());
        }

        await _ticketRepository.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
