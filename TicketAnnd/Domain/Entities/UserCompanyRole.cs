using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Entities;

public class UserCompanyRole:BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; }
    public CompanyRoles Role { get; set; }
    
}