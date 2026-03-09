using System;
using System.Collections.Generic;
using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Sla;

public record GetSlaRulesForActivePolicyQuery(Guid CompanyId) : IRequest<IReadOnlyList<SlaRuleReadModel>>;
public class GetSlaRulesForActivePolicyQueryHandler : IRequestHandler<GetSlaRulesForActivePolicyQuery, IReadOnlyList<SlaRuleReadModel>>
{
    private readonly ISlaPolicyRepository _repo;

    public GetSlaRulesForActivePolicyQueryHandler(ISlaPolicyRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<SlaRuleReadModel>> Handle(GetSlaRulesForActivePolicyQuery request, CancellationToken cancellationToken)
    {
        var rules = await _repo.GetActiveRulesAsync(request.CompanyId, cancellationToken);
        return rules ?? Array.Empty<SlaRuleReadModel>();
    }
}
