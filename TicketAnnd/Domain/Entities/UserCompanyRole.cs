using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Entities;

public class UserCompanyRole:BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; }
    public AppRoles Role { get; set; }
    public UserCompanyRole(Guid id, Guid companyId, Guid userId, AppRoles role)
    {
        Id = id;
        CompanyId = companyId;
        UserId = userId;
        Role = role;
    }
    
}