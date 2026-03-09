using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Infrastructure.Persistence;

namespace TicketAnnd.Application.Teams;

public record SetTeamLeaderCommand(Guid CompanyId, Guid TeamId, Guid CandidateId) : IRequest<Unit>;

public class SetTeamLeaderCommandHandler : IRequestHandler<SetTeamLeaderCommand, Unit>
{
    private readonly ITeamRepository _teamRepo;
    private readonly IUnitOfWork _unitOfWork;

    public SetTeamLeaderCommandHandler(ITeamRepository teamRepo, IUnitOfWork unitOfWork)
    {
        _teamRepo = teamRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SetTeamLeaderCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepo.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null) throw new NotFoundException("Team not found");
        if (team.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to manage this team");

        var member = await _teamRepo.GetMemberAsync(request.TeamId, request.CandidateId, cancellationToken);
        if (member == null) throw new NotFoundException("Team member not found");
        team.SetLeader(member.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
