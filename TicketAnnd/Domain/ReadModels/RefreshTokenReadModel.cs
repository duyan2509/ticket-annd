namespace TicketAnnd.Domain.ReadModels;

public class RefreshTokenReadModel
{
    public string Token { get; init; } = string.Empty;
    public DateTime Expires { get; init; }
    public Guid UserId { get; init; }
    public Guid CompanyId { get; init; }
}
