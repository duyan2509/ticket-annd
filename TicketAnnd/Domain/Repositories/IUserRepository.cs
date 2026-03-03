using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface IUserRepository
{
    Task<LoginUserReadModel?> GetLoginUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserReadModel?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetTrackingByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
