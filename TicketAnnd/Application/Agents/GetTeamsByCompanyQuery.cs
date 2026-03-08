using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Teams;

public record GetTeamsByCompanyQuery(Guid CompanyId) : IRequest<IReadOnlyList<TeamOptionReadModel>>;

public class GetTeamsByCompanyQueryHandler : IRequestHandler<GetTeamsByCompanyQuery, IReadOnlyList<TeamOptionReadModel>>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamsByCompanyQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<IReadOnlyList<TeamOptionReadModel>> Handle(GetTeamsByCompanyQuery request, CancellationToken cancellationToken)
    {
        return await _teamRepository.GetTeamsByCompanyIdAsync(request.CompanyId, cancellationToken);
    }
}
