namespace TicketAnnd.Domain.ReadModels;

public class TicketPagedResultReadModel
{
    public IReadOnlyList<TicketReadModel> Items { get; init; } = Array.Empty<TicketReadModel>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
}
