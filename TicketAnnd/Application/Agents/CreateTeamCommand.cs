using MediatR;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Application.Teams;

public record CreateTeamCommand(Guid CompanyId, string Name) : IRequest<CreateTeamResult>;

public record CreateTeamResult(Guid TeamId, string Name);

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, CreateTeamResult>
{
    private readonly IMediator _mediator;

    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeamCommandHandler(ITeamRepository teamRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _mediator = mediator;
        _teamRepository = teamRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateTeamResult> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = Team.Create(request.CompanyId, request.Name);

        await _teamRepository.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new InvalidateOutputCacheNotification($"company:{request.CompanyId}:teams"), cancellationToken);

        return new CreateTeamResult(team.Id, team.Name);
    }
}
