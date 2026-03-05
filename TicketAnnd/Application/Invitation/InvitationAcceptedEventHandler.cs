using MediatR;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Events;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Invitation;

public class InvitationAcceptedEventHandler 
    : INotificationHandler<InvitationAcceptedEvent>

{
    private readonly IUserCompanyRoleRepository _repo;

    public InvitationAcceptedEventHandler(IUserCompanyRoleRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(InvitationAcceptedEvent notification, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetByUserIdAndCompanyIdAsync(
            notification.UserId,
            notification.CompanyId,
            cancellationToken
        );

        if (existing != null)
            throw new BusinessException("User already exists in company");

        var ucr = new UserCompanyRole(
            Guid.NewGuid(),
            notification.CompanyId,
            notification.UserId,
            notification.Role
        );

        await _repo.AddAsync(ucr, cancellationToken);
    }
}

