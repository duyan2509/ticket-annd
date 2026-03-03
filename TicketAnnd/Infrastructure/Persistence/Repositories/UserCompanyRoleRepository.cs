using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class UserCompanyRoleRepository : IUserCompanyRoleRepository
{
    private readonly TicketAnndDbContext _context;

    public UserCompanyRoleRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(UserCompanyRole userCompanyRole, CancellationToken cancellationToken = default)
    {
        _context.UserCompanyRoles.Add(userCompanyRole);
        return Task.CompletedTask;
    }

    public async Task<bool> HasCompanyAdminRoleAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserCompanyRoles
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.Role == AppRoles.CompanyAdmin, cancellationToken);
    }

    public async Task<UserCompanyRoleReadModel?> GetFirstByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserCompanyRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CompanyId)
            .Select(x => new UserCompanyRoleReadModel
            {
                CompanyId = x.CompanyId,
                Role = x.Role
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserCompanyRoleReadModel?> GetByUserIdAndCompanyIdAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.UserCompanyRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.CompanyId == companyId)
            .Select(x => new UserCompanyRoleReadModel
            {
                CompanyId = x.CompanyId,
                Role = x.Role
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CompanyOptionReadModel>> GetCompanyOptionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserCompanyRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Join(
                _context.Set<Domain.Entities.Company>(),
                ucr => ucr.CompanyId,
                c => c.Id,
                (ucr, c) => new CompanyOptionReadModel
                {
                    CompanyId = ucr.CompanyId,
                    CompanyName = c.Name ?? string.Empty,
                    Role = ucr.Role
                })
            .OrderBy(x => x.CompanyName)
            .ToListAsync(cancellationToken);
    }
}
