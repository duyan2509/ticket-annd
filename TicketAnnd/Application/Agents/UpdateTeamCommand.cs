using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;

namespace TicketAnnd.Application.Teams;

public record UpdateTeamCommand(Guid TeamId, Guid CompanyId, string Name) : IRequest<Unit>;

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, Unit>
{
    private readonly ITeamRepository _teamRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeamCommandHandler(ITeamRepository teamRepo, IUnitOfWork unitOfWork)
    {
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

        return Unit.Value;
    }
}
