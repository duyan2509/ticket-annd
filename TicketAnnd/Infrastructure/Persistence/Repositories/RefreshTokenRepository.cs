using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TicketAnndDbContext _context;

    public RefreshTokenRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshTokenReadModel?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Where(t => t.Token == token)
            .Select(t => new RefreshTokenReadModel
            {
                Token = t.Token,
                Expires = t.Expires,
                UserId = t.UserId,
                CompanyId = t.CompanyId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Add(refreshToken);
        return Task.CompletedTask;
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var entity = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
        if (entity != null)
            _context.RefreshTokens.Remove(entity);
    }
}
