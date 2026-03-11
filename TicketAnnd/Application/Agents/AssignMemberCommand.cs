using MediatR;
using TicketAnnd.Application.Common;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    public AssignMemberCommandHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        IUserCompanyRoleRepository userCompanyRoleRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _userCompanyRoleRepository = userCompanyRoleRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
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

        var exists = await _teamRepository.GetMemberByUserIdAndTeamIdAsync(request.UserId, request.TeamId, cancellationToken);
        if (exists != null)
            return Unit.Value;

        var tm = new TeamMember
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TeamId = request.TeamId
        };

        await _teamRepository.AddAsync(tm, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new InvalidateOutputCacheNotification("Teams"), cancellationToken);

        return Unit.Value;
    }
}
