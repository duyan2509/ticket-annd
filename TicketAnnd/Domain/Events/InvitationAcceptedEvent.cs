using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Events;

public record InvitationAcceptedEvent(
    Guid InvitationId,
    Guid CompanyId,
    Guid UserId,
    AppRoles Role
) : IDomainEvent;
