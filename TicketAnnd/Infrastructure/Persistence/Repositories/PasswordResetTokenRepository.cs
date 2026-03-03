using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly TicketAnndDbContext _context;

    public PasswordResetTokenRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordResetTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
    }

    public Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
    {
        _context.PasswordResetTokens.Add(token);
        return Task.CompletedTask;
    }

    public async Task RevokeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var toRemove = await _context.PasswordResetTokens
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
        _context.PasswordResetTokens.RemoveRange(toRemove);
    }
}
