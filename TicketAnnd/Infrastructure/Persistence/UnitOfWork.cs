using TicketAnnd.Domain;

namespace TicketAnnd.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly TicketAnndDbContext _context;

    public UnitOfWork(TicketAnndDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
