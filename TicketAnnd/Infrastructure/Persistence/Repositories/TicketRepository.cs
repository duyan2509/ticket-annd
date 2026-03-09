using MediatR;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class TicketRepository:ITicketRepository
{
    private readonly TicketAnndDbContext _context;

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
    }
}