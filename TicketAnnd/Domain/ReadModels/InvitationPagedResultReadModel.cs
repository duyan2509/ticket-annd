namespace TicketAnnd.Domain.ReadModels;

public class InvitationPagedResultReadModel
{
    public IReadOnlyList<InvitationReadModel> Items { get; init; } = Array.Empty<InvitationReadModel>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
}
