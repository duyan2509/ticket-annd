using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;
using TicketAnnd.Application;

namespace TicketAnnd.Application.Sla;

public record ActivateSlaPolicyCommand(Guid PolicyId, Guid CompanyId) : IRequest<Unit>;

public class ActivateSlaPolicyCommandHandler : IRequestHandler<ActivateSlaPolicyCommand, Unit>
{
    private readonly IMediator _mediator;

    private readonly ISlaPolicyRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateSlaPolicyCommandHandler(ISlaPolicyRepository repo, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _mediator = mediator;
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActivateSlaPolicyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetPolicyByIdAsync(request.PolicyId, cancellationToken);
        if (existing == null) throw new NotFoundException("SLA policy not found");
        if (existing.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to activate this SLA policy");
        if (existing.IsActive) throw new BadRequestException("Policy already activated");
        await _repo.SetActiveAsync(request.PolicyId, request.CompanyId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new InvalidateOutputCacheNotification("Sla"), cancellationToken);
        return Unit.Value;
    }
}
