namespace TicketAnnd.Domain.ReadModels;

public class CompanyPagedResultReadModel
{
    public IReadOnlyList<CompanyPagedItemReadModel> Items { get; init; } = Array.Empty<CompanyPagedItemReadModel>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
}
