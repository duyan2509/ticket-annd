namespace TicketAnnd.Domain.ReadModels;

public class LoginUserReadModel
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsAdmin { get; init; }
}
