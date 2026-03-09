using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ITeamRepository
{
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task<Team?> GetByIdAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeamOptionReadModel>> GetTeamsByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<TeamMember> GetMemberAsync(Guid requestTeamId, Guid requestCandidateId, CancellationToken cancellationToken);

    Task AddAsync(TeamMember teamMember, CancellationToken cancellationToken = default);
    Task<TeamMember?> GetMemberByUserIdAndTeamIdAsync(Guid userId, Guid teamId, CancellationToken cancellationToken = default);
    Task<MemberPagedResultReadModel> GetMembersByTeamIdAsync(Guid teamId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
