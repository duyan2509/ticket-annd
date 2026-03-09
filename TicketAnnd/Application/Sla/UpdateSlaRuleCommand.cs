using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;
using TicketAnnd.Application;

namespace TicketAnnd.Application.Sla;

public record UpdateSlaRuleCommand(Guid RuleId, Guid CompanyId, string? Name, int? FirstResponseMinutes, int? ResolutionMinutes) : IRequest<Unit>;

public class UpdateSlaRuleCommandHandler : IRequestHandler<UpdateSlaRuleCommand, Unit>
{
    private readonly ISlaPolicyRepository _policyRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSlaRuleCommandHandler(ISlaPolicyRepository policyRepo, IUnitOfWork unitOfWork)
    {
        _policyRepo = policyRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateSlaRuleCommand request, CancellationToken cancellationToken)
    {
        var existing = await _policyRepo.GetRuleByIdAsync(request.RuleId, cancellationToken);
        if (existing == null) throw new NotFoundException("SLA rule not found");

        var policy = await _policyRepo.GetPolicyByIdAsync(existing.SlaPolicyId, cancellationToken);
        if (policy == null) throw new NotFoundException("SLA policy not found");
        if (policy.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to update rule for this SLA policy");

        existing.FirstResponseMinutes = request?.FirstResponseMinutes ?? existing.FirstResponseMinutes; ;
        existing.ResolutionMinutes = request?.ResolutionMinutes ?? existing.ResolutionMinutes;
        existing.Name = request?.Name ?? existing.Name;
        await _policyRepo.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
