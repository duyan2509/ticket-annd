using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Entities;

public class Invitation:BaseEntity
{
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; }
    public string Email { get; set; }
    public DateTime Expires { get; set; }
    public InviationStatuses Status { get; set; } = InviationStatuses.Pending;
    public CompanyRoles Role { get; set; } = CompanyRoles.Customer;
}
