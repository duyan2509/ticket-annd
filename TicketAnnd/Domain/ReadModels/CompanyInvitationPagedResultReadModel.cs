namespace TicketAnnd.Domain.ReadModels;

public class CompanyInvitationPagedResultReadModel
{
    public IReadOnlyList<CompanyInvitationItemReadModel> Items { get; init; } = Array.Empty<CompanyInvitationItemReadModel>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
}
