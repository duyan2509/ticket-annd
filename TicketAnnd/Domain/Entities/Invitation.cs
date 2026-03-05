using TicketAnnd.Application;
using TicketAnnd.Application.Invitation;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Events;

namespace TicketAnnd.Domain.Entities;

public class Invitation:BaseEntity
{
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; }
    public string Email { get; set; }
    public DateTime Expires { get; set; }
    public InviationStatuses Status { get; set; } = InviationStatuses.Pending;
    public AppRoles Role { get; set; } = AppRoles.Customer;
    public DateTime? ResponseAt { get; set; }
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }

    public bool IsExpires()
    {
        return DateTime.UtcNow > Expires;
    }
    public void Respond(Guid responderUserId, InviationStatuses newStatus)
    {
        if (IsExpires())
            throw new BusinessException("Invitation expired");

        if (Status != InviationStatuses.Pending)
            throw new BusinessException("Invitation already responded");

        Status = newStatus;
        ResponseAt = DateTime.UtcNow;

        if (UserId is null)
            UserId = responderUserId;

        if (Status == InviationStatuses.Accepted)
        {
            AddDomainEvent(new InvitationAcceptedEvent(
                Id,
                CompanyId,
                responderUserId,
                Role
            ));
        }
    
    }


}
