using System;
using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;
using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Application.Tickets;

public record PauseTicketCommand(Guid CompanyId, Guid TicketId, string PauseType, string Reason, DateTime? ResumeAt, Guid ActorId) : IRequest<Guid>;

public class PauseTicketCommandHandler : IRequestHandler<PauseTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public PauseTicketCommandHandler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(PauseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTrackingByIdCompanyIdAsync(request.TicketId, request.CompanyId, cancellationToken);
        if (ticket is null)
            throw new BadRequestException("Ticket not found in company");

        ticket.Pause(request.ActorId, request.PauseType, request.Reason, request.ResumeAt);

        await _ticketRepository.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
