using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Domain.Repositories;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
}