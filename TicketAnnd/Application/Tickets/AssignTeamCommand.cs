using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Tickets;

public record AssignTeamCommand(Guid CompanyId, Guid TicketId, Guid TeamId) : IRequest<Guid>;

public class AsssignTeamCommandHandler : IRequestHandler<AssignTeamCommand,Guid>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AsssignTeamCommandHandler(ITeamRepository teamRepository, ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AssignTeamCommand request, CancellationToken cancellationToken)
    {
        bool validTeam = await _teamRepository.CheckExistInCompanyAsync(request.CompanyId, request.TeamId, cancellationToken);
        if(!validTeam)
            throw new BadRequestException("Team not found in company");
        var ticket = await _ticketRepository.GetTrackingByIdCompanyIdAsync(request.TicketId, request.CompanyId, cancellationToken);
        if(ticket is null)
            throw new BadRequestException("Ticket not found in company");
        ticket.TeamId = request.TeamId;
        await _ticketRepository.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ticket.Id;
    }
}