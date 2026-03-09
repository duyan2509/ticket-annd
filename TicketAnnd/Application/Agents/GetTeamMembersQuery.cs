using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Teams;

public record GetTeamMembersQuery(Guid TeamId, int Page = 1, int Size = 10) : IRequest<MemberPagedResultReadModel>;

public class GetTeamMembersQueryHandler : IRequestHandler<GetTeamMembersQuery, MemberPagedResultReadModel>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamMembersQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<MemberPagedResultReadModel> Handle(GetTeamMembersQuery request, CancellationToken cancellationToken)
    {
        return await _teamRepository.GetMembersByTeamIdAsync(request.TeamId, request.Page, request.Size, cancellationToken);
    }
}
