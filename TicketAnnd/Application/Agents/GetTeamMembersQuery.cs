using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Teams;

public record GetTeamMembersQuery(Guid TeamId, int Page = 1, int Size = 10) : IRequest<MemberPagedResultReadModel>;

public class GetTeamMembersQueryHandler : IRequestHandler<GetTeamMembersQuery, MemberPagedResultReadModel>
{
    private readonly ITeamMemberRepository _teamMemberRepository;

    public GetTeamMembersQueryHandler(ITeamMemberRepository teamMemberRepository)
    {
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<MemberPagedResultReadModel> Handle(GetTeamMembersQuery request, CancellationToken cancellationToken)
    {
        return await _teamMemberRepository.GetMembersByTeamIdAsync(request.TeamId, request.Page, request.Size, cancellationToken);
    }
}
