using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain;
using TicketAnnd.Application;

namespace TicketAnnd.Application.Sla;

public record CreateSlaRuleCommand(Guid SlaPolicyId, Guid CompanyId, string Name, int FirstResponseMinutes, int ResolutionMinutes) : IRequest<Guid>;

public class CreateSlaRuleCommandHandler : IRequestHandler<CreateSlaRuleCommand, Guid>
{
    private readonly ISlaRuleRepository _ruleRepo;
    private readonly ISlaPolicyRepository _policyRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSlaRuleCommandHandler(ISlaRuleRepository ruleRepo, ISlaPolicyRepository policyRepo, IUnitOfWork unitOfWork)
    {
        _ruleRepo = ruleRepo;
        _policyRepo = policyRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSlaRuleCommand request, CancellationToken cancellationToken)
    {
        var policy = await _policyRepo.GetByIdAsync(request.SlaPolicyId, cancellationToken);
        if (policy == null) throw new NotFoundException("SLA policy not found");
        if (policy.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to create rule for this SLA policy");

        var rule = new SlaRule { Id = Guid.NewGuid(), Name= request.Name, SlaPolicyId = request.SlaPolicyId, FirstResponseMinutes = request.FirstResponseMinutes, ResolutionMinutes = request.ResolutionMinutes };
        await _ruleRepo.AddAsync(rule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return rule.Id;
    }
}
