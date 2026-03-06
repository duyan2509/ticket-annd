using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;
using TicketAnnd.Application;

namespace TicketAnnd.Application.Sla;

public record UpdateSlaPolicyCommand(Guid PolicyId, Guid CompanyId, string Name) : IRequest<Unit>;

public class UpdateSlaPolicyCommandHandler : IRequestHandler<UpdateSlaPolicyCommand, Unit>
{
    private readonly ISlaPolicyRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSlaPolicyCommandHandler(ISlaPolicyRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateSlaPolicyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetByIdAsync(request.PolicyId, cancellationToken);
        if (existing == null) throw new NotFoundException("SLA policy not found");
        if (existing.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to update this SLA policy");

        existing.Name = request.Name;
        await _repo.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
