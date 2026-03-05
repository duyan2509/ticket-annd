using TicketAnnd.Domain.Enums;

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

}
