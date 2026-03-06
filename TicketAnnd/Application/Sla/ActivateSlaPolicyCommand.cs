using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;
using TicketAnnd.Application;

namespace TicketAnnd.Application.Sla;

public record ActivateSlaPolicyCommand(Guid PolicyId, Guid CompanyId) : IRequest<Unit>;

public class ActivateSlaPolicyCommandHandler : IRequestHandler<ActivateSlaPolicyCommand, Unit>
{
    private readonly ISlaPolicyRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateSlaPolicyCommandHandler(ISlaPolicyRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActivateSlaPolicyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetByIdAsync(request.PolicyId, cancellationToken);
        if (existing == null) throw new NotFoundException("SLA policy not found");
        if (existing.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to activate this SLA policy");

        await _repo.SetActiveAsync(request.PolicyId, request.CompanyId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
