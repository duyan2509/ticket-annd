using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenReadModel?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(string token, CancellationToken cancellationToken = default);
}
