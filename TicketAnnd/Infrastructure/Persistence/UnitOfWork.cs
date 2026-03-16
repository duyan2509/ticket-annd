using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TicketAnnd.Domain;

namespace TicketAnnd.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly TicketAnndDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(TicketAnndDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict when saving changes to the database. The affected entity may have been modified or deleted.");
            return 0;
        }
    }
}
