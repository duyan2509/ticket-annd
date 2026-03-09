using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Sla;

public record GetSlaRulesByPolicyQuery(Guid SlaPolicyId) : IRequest<IReadOnlyList<SlaRuleReadModel>>;

public class GetSlaRulesByPolicyQueryHandler : IRequestHandler<GetSlaRulesByPolicyQuery, IReadOnlyList<SlaRuleReadModel>>
{
    private readonly ISlaPolicyRepository _policyRepo;

    public GetSlaRulesByPolicyQueryHandler(ISlaPolicyRepository policyRepo)
    {
        _policyRepo=policyRepo;
    }

    public async Task<IReadOnlyList<SlaRuleReadModel>> Handle(GetSlaRulesByPolicyQuery request, CancellationToken cancellationToken)
    {
        return await _policyRepo.GetRulesByPolicyIdAsync(request.SlaPolicyId, cancellationToken);
    }
}
