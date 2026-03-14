using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Tickets;

public record GetTicketsQuery(Guid CompanyId, int Page = 1, int PageSize = 10, Guid? TeamId = null, string? Status = null, Guid? CategoryId = null, string? Subject = null, bool? Unassigned = null) : IRequest<TicketPagedResultReadModel>;

public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, TicketPagedResultReadModel>
{
    private readonly ITicketRepository _ticketRepository;

    public GetTicketsQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketPagedResultReadModel> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        return await _ticketRepository.GetPagedByCompanyIdAsync(request.CompanyId, request.Page, request.PageSize, request.TeamId, request.Status, request.CategoryId, request.Subject, request.Unassigned, cancellationToken);
    }
}
