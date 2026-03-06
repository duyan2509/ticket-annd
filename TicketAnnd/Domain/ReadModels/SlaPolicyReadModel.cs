namespace TicketAnnd.Domain.ReadModels;

public class SlaPolicyReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyList<SlaRuleReadModel> Rules { get; init; } = Array.Empty<SlaRuleReadModel>();
}
