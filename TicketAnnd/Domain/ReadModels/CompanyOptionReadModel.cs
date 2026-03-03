using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.ReadModels;

public class CompanyOptionReadModel
{
    public Guid CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public AppRoles Role { get; init; }
}
