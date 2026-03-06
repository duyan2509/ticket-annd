using TicketAnnd.Domain;

namespace TicketAnnd.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly TicketAnndDbContext _context;

    public UnitOfWork(TicketAnndDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
