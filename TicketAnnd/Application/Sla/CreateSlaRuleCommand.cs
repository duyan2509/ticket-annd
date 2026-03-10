using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain;
using TicketAnnd.Application;

namespace TicketAnnd.Application.Sla;

public record CreateSlaRuleCommand(Guid SlaPolicyId, Guid CompanyId, string Name, int FirstResponseMinutes, int ResolutionMinutes) : IRequest<Guid>;

public class CreateSlaRuleCommandHandler : IRequestHandler<CreateSlaRuleCommand, Guid>
{
    private readonly ISlaPolicyRepository _policyRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSlaRuleCommandHandler(ISlaPolicyRepository policyRepo, IUnitOfWork unitOfWork)
    {
        _policyRepo = policyRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSlaRuleCommand request, CancellationToken cancellationToken)
    {
        var policy = await _policyRepo.GetPolicyByIdAsync(request.SlaPolicyId, cancellationToken);
        if (policy == null) throw new NotFoundException("SLA policy not found");
        if (policy.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to create rule for this SLA policy");

        var rule = new SlaRule { Name= request.Name, SlaPolicyId = request.SlaPolicyId, FirstResponseMinutes = request.FirstResponseMinutes, ResolutionMinutes = request.ResolutionMinutes };
        policy.AddRule(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return rule.Id;
    }
}
