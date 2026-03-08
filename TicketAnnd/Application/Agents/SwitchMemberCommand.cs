using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;

namespace TicketAnnd.Application.Teams;

public record SwitchMemberCommand(Guid CompanyId, Guid UserId, Guid FromTeamId, Guid ToTeamId) : IRequest<Unit>;

public class SwitchMemberCommandHandler : IRequestHandler<SwitchMemberCommand,Unit>
{
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SwitchMemberCommandHandler(ITeamMemberRepository teamMemberRepository, ITeamRepository teamRepository, IUnitOfWork unitOfWork)
    {
        _teamMemberRepository = teamMemberRepository;
        _teamRepository = teamRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SwitchMemberCommand request, CancellationToken cancellationToken)
    {
        var tm = await _teamMemberRepository.GetByUserIdAndTeamIdAsync(request.UserId, request.FromTeamId, cancellationToken);
        if (tm == null)
            throw new BadRequestException("User is not member of source team");

        var toTeam = await _teamRepository.GetByIdAsync(request.ToTeamId, cancellationToken);
        if (toTeam == null)
            throw new BadRequestException("Target team not found");
        if(toTeam.CompanyId != request.CompanyId)
            throw new BadRequestException("Target team id not in current company");
        var fromTeam = await _teamRepository.GetByIdAsync(request.FromTeamId, cancellationToken);
        if (fromTeam == null || fromTeam.CompanyId != toTeam.CompanyId)
            throw new BadRequestException("Teams must belong to same company");

        tm.TeamId = request.ToTeamId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

}
