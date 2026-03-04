using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.ReadModels;

public class CompanyPagedItemReadModel
{
    public Guid CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string Role { get; init; }
}
