using MediatR;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Invitation;

public record GetInvitationsByCompanyQuery(Guid CompanyId, int Page = 1, int Size = 50) : IRequest<CompanyInvitationPagedResultReadModel>;

public class GetInvitationsByCompanyQueryHandler : IRequestHandler<GetInvitationsByCompanyQuery, CompanyInvitationPagedResultReadModel>
{
    private readonly IInvitationRepository _invitationRepository;

    public GetInvitationsByCompanyQueryHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<CompanyInvitationPagedResultReadModel> Handle(GetInvitationsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var result = await _invitationRepository.GetByCompanyIdAsync(request.CompanyId, request.Page, request.Size, cancellationToken);
        return result;
    }
}
