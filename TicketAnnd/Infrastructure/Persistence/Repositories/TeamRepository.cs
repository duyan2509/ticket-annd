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
}
