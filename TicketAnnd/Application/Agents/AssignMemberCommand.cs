using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Application.Teams;

public record AssignMemberCommand(Guid CompanyId, Guid TeamId, Guid UserId) : IRequest<Unit>;

public class AssignMemberCommandHandler : IRequestHandler<AssignMemberCommand,Unit>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignMemberCommandHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        IUserCompanyRoleRepository userCompanyRoleRepository,
        ITeamMemberRepository teamMemberRepository,
        IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _userCompanyRoleRepository = userCompanyRoleRepository;
        _teamMemberRepository = teamMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AssignMemberCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null)
            throw new BadRequestException("Team not found");
        if(team.CompanyId != request.CompanyId)
            throw new BadRequestException("Target team id not in current company");
        var user = await _userRepository.GetTrackingByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new BadRequestException("User not found");

        var role = await _userCompanyRoleRepository.GetByUserIdAndCompanyIdAsync(request.UserId, team.CompanyId, cancellationToken);
        if (role == null)
            throw new BadRequestException("User not in company");

        var exists = await _teamMemberRepository.GetByUserIdAndTeamIdAsync(request.UserId, request.TeamId, cancellationToken);
        if (exists != null)
            return Unit.Value;

        var tm = new TeamMember
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TeamId = request.TeamId
        };

        await _teamMemberRepository.AddAsync(tm, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
