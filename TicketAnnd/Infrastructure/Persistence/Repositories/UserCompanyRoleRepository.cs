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
            .Join(
                _context.Set<Company>(),
                ucr => ucr.CompanyId,
                c => c.Id,
                (ucr, c) => new { ucr, c })
            .Select(x => new UserCompanyRoleReadModel
            {
                CompanyId = x.ucr.CompanyId,
                Role = x.ucr.Role,
                CompanyName = x.c.Name ?? string.Empty
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserCompanyRoleReadModel?> GetByUserIdAndCompanyIdAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.UserCompanyRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.CompanyId == companyId)
            .Join(
                _context.Set<Company>(),
                ucr => ucr.CompanyId,
                c => c.Id,
                (ucr, c) => new { ucr, c })
            .Select(x => new UserCompanyRoleReadModel
            {
                CompanyId = x.ucr.CompanyId,
                Role = x.ucr.Role,
                CompanyName = x.c.Name ?? string.Empty
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CompanyOptionReadModel>> GetCompanyOptionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserCompanyRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Join(
                _context.Set<Company>(),
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

    public async Task<CompanyPagedResultReadModel> GetCompanyOptionsPagedByUserIdAsync(Guid userId, int page  = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var baseQuery = _context.UserCompanyRoles.AsNoTracking().Where(x => x.UserId == userId);
        var total = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Join(
                _context.Set<Company>(),
                ucr => ucr.CompanyId,
                c => c.Id,
                (ucr, c) => new { ucr, c })
            .OrderBy(x => x.c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CompanyPagedItemReadModel
            {
                CompanyId = x.ucr.CompanyId,
                CompanyName = x.c.Name ?? string.Empty,
                Role = x.ucr.Role.ToString()
            })
            .ToListAsync(cancellationToken);

        return new CompanyPagedResultReadModel
        {
            Items = items,
            Total = total,
            Page = page,
            Size = pageSize,
        };
    }

    public async Task<MemberPagedResultReadModel> GetMembersByCompanyIdAsync(Guid companyId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var baseQuery = _context.UserCompanyRoles.AsNoTracking().Where(x => x.CompanyId == companyId);
        var total = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Join(
                _context.Set<User>(),
                ucr => ucr.UserId,
                u => u.Id,
                (ucr, u) => new { ucr, u })
            .OrderBy(x => x.u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new MemberPagedItemReadModel
            {
                UserId = x.u.Id,
                Email = x.u.Email,
                Role = x.ucr.Role.ToString()
            })
            .ToListAsync(cancellationToken);

        return new MemberPagedResultReadModel
        {
            Items = items,
            Total = total,
            Page = page,
            Size = pageSize,
        };
    }
}
