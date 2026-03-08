using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ITeamRepository
{
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task<Team?> GetByIdAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeamOptionReadModel>> GetTeamsByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
}
