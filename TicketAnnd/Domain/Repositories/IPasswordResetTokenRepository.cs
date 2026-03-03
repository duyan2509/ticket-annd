using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Domain.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
    Task RevokeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
