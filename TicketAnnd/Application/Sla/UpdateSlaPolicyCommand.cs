using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;
using TicketAnnd.Application;

namespace TicketAnnd.Application.Sla;

public record UpdateSlaPolicyCommand(Guid PolicyId, Guid CompanyId, string Name) : IRequest<Unit>;

public class UpdateSlaPolicyCommandHandler : IRequestHandler<UpdateSlaPolicyCommand, Unit>
{
    private readonly IMediator _mediator;

    private readonly ISlaPolicyRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSlaPolicyCommandHandler(ISlaPolicyRepository repo, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _mediator = mediator;
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateSlaPolicyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetPolicyByIdAsync(request.PolicyId, cancellationToken);
        if (existing == null) throw new NotFoundException("SLA policy not found");
        if (existing.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to update this SLA policy");

        existing.Name = request.Name;
        await _repo.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new InvalidateOutputCacheNotification("Sla"), cancellationToken);
        return Unit.Value;
    }
}
