namespace TicketAnnd.Domain.ReadModels;

public class TeamOptionReadModel
{
    public Guid TeamId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Size { get; init; }
}
