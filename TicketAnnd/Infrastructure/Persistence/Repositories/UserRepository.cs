using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TicketAnndDbContext _context;

    public UserRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public async Task<LoginUserReadModel?> GetLoginUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Email == email)
            .Select(u => new LoginUserReadModel
            {
                Id = u.Id,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                IsActive = u.IsActive,
                IsAdmin = u.IsSuperAdmin
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserReadModel?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserReadModel
            {
                Id = u.Id,
                Email = u.Email,
                IsActive = u.IsActive,
                IsSuperAdmin = u.IsSuperAdmin
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetTrackingByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }
}
