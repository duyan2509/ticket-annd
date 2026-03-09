namespace TicketAnnd.Domain.Entities;

public class SlaPolicy:BaseEntity
{
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<SlaRule> SlaRules { get; set; } = new List<SlaRule>();

    public void AddRule(SlaRule rule)
    {
        SlaRules.Add(rule);
    }
}