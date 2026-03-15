using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;

namespace TicketAnnd.Application.Teams;

public record UpdateTeamCommand(Guid TeamId, Guid CompanyId, string Name) : IRequest<Unit>;

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, Unit>
{
    private readonly IMediator _mediator;

    private readonly ITeamRepository _teamRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeamCommandHandler(ITeamRepository teamRepo, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _mediator = mediator;
        _teamRepo = teamRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepo.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null) throw new NotFoundException("Team not found");
        if (team.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to manage this team");

        team.Name = request.Name?.Trim() ?? string.Empty;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new InvalidateOutputCacheNotification($"company:{request.CompanyId}:teams"), cancellationToken);

        return Unit.Value;
    }
}
