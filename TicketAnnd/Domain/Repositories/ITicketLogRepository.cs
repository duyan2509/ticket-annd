using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ITicketLogRepository
{
    Task InsertAsync(TicketLogDocument doc, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketLogDocument>> GetByTicketAsync(Guid ticketId, int page = 1, int size = 50, CancellationToken cancellationToken = default);
}
