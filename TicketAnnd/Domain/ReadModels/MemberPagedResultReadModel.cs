namespace TicketAnnd.Domain.ReadModels;

public class MemberPagedResultReadModel
{
    public IReadOnlyList<MemberPagedItemReadModel> Items { get; init; } = Array.Empty<MemberPagedItemReadModel>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
}
