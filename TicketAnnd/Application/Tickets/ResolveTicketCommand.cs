using System;
using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;
using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Application.Tickets;

public record ResolveTicketCommand(Guid CompanyId, Guid TicketId, Guid ActorId, string? Note) : IRequest<Guid>;

public class ResolveTicketCommandHandler : IRequestHandler<ResolveTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public ResolveTicketCommandHandler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(ResolveTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTrackingByIdCompanyIdAsync(request.TicketId, request.CompanyId, cancellationToken);
        if (ticket is null)
            throw new BadRequestException("Ticket not found in company");

        ticket.Resolve(request.ActorId, request.Note);

        await _ticketRepository.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
