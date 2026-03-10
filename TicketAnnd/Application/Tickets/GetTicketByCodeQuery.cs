using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Tickets;

public record GetTicketByCodeQuery(Guid CompanyId, string TicketCode) : IRequest<TicketReadModel?>;

public class GetTicketByCodeQueryHandler : IRequestHandler<GetTicketByCodeQuery, TicketReadModel?>
{
    private readonly ITicketRepository _ticketRepository;

    public GetTicketByCodeQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketReadModel?> Handle(GetTicketByCodeQuery request, CancellationToken cancellationToken)
    {
        return await _ticketRepository.GetByCompanyIdAndCodeAsync(request.CompanyId, request.TicketCode, cancellationToken);
    }
}
