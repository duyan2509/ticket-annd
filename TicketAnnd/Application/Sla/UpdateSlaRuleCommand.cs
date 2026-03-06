using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;
using TicketAnnd.Application;

namespace TicketAnnd.Application.Sla;

public record UpdateSlaRuleCommand(Guid RuleId, Guid CompanyId, string? Name, int? FirstResponseMinutes, int? ResolutionMinutes) : IRequest<Unit>;

public class UpdateSlaRuleCommandHandler : IRequestHandler<UpdateSlaRuleCommand, Unit>
{
    private readonly ISlaRuleRepository _ruleRepo;
    private readonly ISlaPolicyRepository _policyRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSlaRuleCommandHandler(ISlaRuleRepository ruleRepo, ISlaPolicyRepository policyRepo, IUnitOfWork unitOfWork)
    {
        _ruleRepo = ruleRepo;
        _policyRepo = policyRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateSlaRuleCommand request, CancellationToken cancellationToken)
    {
        var existing = await _ruleRepo.GetByIdAsync(request.RuleId, cancellationToken);
        if (existing == null) throw new NotFoundException("SLA rule not found");

        var policy = await _policyRepo.GetByIdAsync(existing.SlaPolicyId, cancellationToken);
        if (policy == null) throw new NotFoundException("SLA policy not found");
        if (policy.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to update rule for this SLA policy");

        existing.FirstResponseMinutes = request?.FirstResponseMinutes ?? existing.FirstResponseMinutes; ;
        existing.ResolutionMinutes = request?.ResolutionMinutes ?? existing.ResolutionMinutes;
        existing.Name = request?.Name ?? existing.Name;
        await _ruleRepo.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
