using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly TicketAnndDbContext _context;

    public TeamMemberRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(TeamMember teamMember, CancellationToken cancellationToken = default)
    {
        _context.TeamMembers.Add(teamMember);
        return Task.CompletedTask;
    }

    public async Task<TeamMember?> GetByUserIdAndTeamIdAsync(Guid userId, Guid teamId, CancellationToken cancellationToken = default)
    {
        return await _context.TeamMembers.FirstOrDefaultAsync(x => x.UserId == userId && x.TeamId == teamId, cancellationToken);
    }

    public async Task<MemberPagedResultReadModel> GetMembersByTeamIdAsync(Guid teamId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var baseQuery = _context.TeamMembers.AsNoTracking().Where(x => x.TeamId == teamId);
        var total = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Join(
                _context.Set<User>(),
                ua => ua.UserId,
                u => u.Id,
                (ua, u) => new { ua, u })
            .OrderBy(x => x.u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new MemberPagedItemReadModel
            {
                UserId = x.u.Id,
                Email = x.u.Email,
                Role = string.Empty
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
