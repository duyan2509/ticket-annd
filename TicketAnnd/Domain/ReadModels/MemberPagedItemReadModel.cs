namespace TicketAnnd.Domain.ReadModels;

public class MemberPagedItemReadModel
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public Guid? TeamId { get; init; }
    public string? TeamName { get; init; } = string.Empty;
}
