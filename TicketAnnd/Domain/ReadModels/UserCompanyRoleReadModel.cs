using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.ReadModels;

public class UserCompanyRoleReadModel
{
    public Guid CompanyId { get; init; }
    public AppRoles Role { get; init; }
}
