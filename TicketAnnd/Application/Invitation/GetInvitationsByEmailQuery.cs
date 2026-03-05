using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Invitation;

public record GetInvitationsByEmailQuery(string Email, InviationStatuses? Status, string? CompanyName, int Page = 1, int Size = 50) : IRequest<IEnumerable<InvitationReadModel>>;

public class GetInvitationsByEmailQueryHandler : IRequestHandler<GetInvitationsByEmailQuery, IEnumerable<InvitationReadModel>>
{
    private readonly IInvitationRepository _invitationRepository;

    public GetInvitationsByEmailQueryHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<IEnumerable<InvitationReadModel>> Handle(GetInvitationsByEmailQuery request, CancellationToken cancellationToken)
    {
        var items = await _invitationRepository.GetByEmailAsync(request.Email, request.Status, request.CompanyName, request.Page, request.Size, cancellationToken);
        return items;
    }
}
