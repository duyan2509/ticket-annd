namespace TicketAnnd.Domain.ReadModels;

public class SlaPolicyPagedResultReadModel
{
    public IReadOnlyList<SlaPolicyReadModel> Items { get; init; } = Array.Empty<SlaPolicyReadModel>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
}
