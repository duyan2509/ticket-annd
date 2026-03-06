using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Sla;

public record GetSlaRulesByPolicyQuery(Guid SlaPolicyId) : IRequest<IReadOnlyList<SlaRuleReadModel>>;

public class GetSlaRulesByPolicyQueryHandler : IRequestHandler<GetSlaRulesByPolicyQuery, IReadOnlyList<SlaRuleReadModel>>
{
    private readonly ISlaRuleRepository _ruleRepo;

    public GetSlaRulesByPolicyQueryHandler(ISlaRuleRepository ruleRepo)
    {
        _ruleRepo = ruleRepo;
    }

    public async Task<IReadOnlyList<SlaRuleReadModel>> Handle(GetSlaRulesByPolicyQuery request, CancellationToken cancellationToken)
    {
        return await _ruleRepo.GetByPolicyIdAsync(request.SlaPolicyId, cancellationToken);
    }
}
