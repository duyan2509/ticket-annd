namespace TicketAnnd.Domain.ReadModels;

public class SlaRuleReadModel
{
    public Guid Id { get; init; }
    public int FirstResponseMinutes { get; init; }
    public int ResolutionMinutes { get; init; }
    public string Name { get; init; }
}
