namespace TicketAnnd.Domain.ReadModels;

public class UserReadModel
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
