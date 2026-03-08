using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;

namespace TicketAnnd.Domain.Repositories;

public interface ITeamMemberRepository
{
    Task AddAsync(TeamMember teamMember, CancellationToken cancellationToken = default);
    Task<TeamMember?> GetByUserIdAndTeamIdAsync(Guid userId, Guid teamId, CancellationToken cancellationToken = default);
    Task<MemberPagedResultReadModel> GetMembersByTeamIdAsync(Guid teamId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
