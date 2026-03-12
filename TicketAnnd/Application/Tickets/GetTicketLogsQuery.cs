using System;
using System.Collections.Generic;
using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Tickets;

public record GetTicketLogsQuery(Guid CompanyId, Guid TicketId, int Page = 1, int Size = 50) : IRequest<IReadOnlyList<TicketLogDocument>>;

public class GetTicketLogsQueryHandler : IRequestHandler<GetTicketLogsQuery, IReadOnlyList<TicketLogDocument>>
{
    private readonly ITicketLogRepository _repository;

    public GetTicketLogsQueryHandler(ITicketLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TicketLogDocument>> Handle(GetTicketLogsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByTicketAsync(request.TicketId, request.Page, request.Size, cancellationToken);
    }
}
