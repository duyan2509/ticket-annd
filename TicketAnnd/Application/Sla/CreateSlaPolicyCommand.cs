using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Sla;

public record CreateSlaPolicyCommand(Guid CompanyId, string Name) : IRequest<Guid>;

public class CreateSlaPolicyCommandHandler : IRequestHandler<CreateSlaPolicyCommand, Guid>
{
    private readonly IMediator _mediator;

    private readonly ISlaPolicyRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSlaPolicyCommandHandler(ISlaPolicyRepository repo, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _mediator = mediator;
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSlaPolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = new TicketAnnd.Domain.Entities.SlaPolicy { Id = Guid.NewGuid(), CompanyId = request.CompanyId, Name = request.Name, IsActive = false };
        await _repo.AddAsync(policy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new InvalidateOutputCacheNotification("Sla"), cancellationToken);
        return policy.Id;
    }
}
