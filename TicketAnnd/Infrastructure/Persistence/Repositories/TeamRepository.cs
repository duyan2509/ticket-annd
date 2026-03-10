using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly TicketAnndDbContext _context;

    public TeamRepository(TicketAnndDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Add(team);
        return Task.CompletedTask;
    }

    public async Task<Team?> GetByIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Teams.FindAsync(new object[] { teamId }, cancellationToken);
    }

    public async Task<IReadOnlyList<TeamOptionReadModel>> GetTeamsByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId)
            .Select(a => new TeamOptionReadModel
            {
                TeamId = a.Id,
                Name = a.Name ?? string.Empty,
                Size = a.TeamMembers.Count
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<TeamMember> GetMemberAsync(Guid requestTeamId, Guid requestCandidateId, CancellationToken cancellationToken)
    {
        var member = _context.TeamMembers.FirstOrDefault(tm => tm.TeamId == requestTeamId && tm.UserId == requestCandidateId);
        return Task.FromResult(member);
    }

    public Task AddAsync(TeamMember teamMember, CancellationToken cancellationToken = default)
    {
        _context.TeamMembers.Add(teamMember);
        return Task.CompletedTask;
    }

    public async Task<TeamMember?> GetMemberByUserIdAndTeamIdAsync(Guid userId, Guid teamId, CancellationToken cancellationToken = default)
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
                Role = string.Empty,
                IsLeader = x.ua.TeamId == teamId && x.ua.UserId == x.u.Id && x.ua.Team.LeaderId == x.ua.Id
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

    public Task<bool> CheckExistInCompanyAsync(Guid requestCompanyId, Guid requestTeamId, CancellationToken cancellationToken = default)
    {
        return _context.Teams
            .AsNoTracking()
            .AnyAsync(t=>t.Id == requestTeamId && t.CompanyId == requestCompanyId, cancellationToken);
    }
}
