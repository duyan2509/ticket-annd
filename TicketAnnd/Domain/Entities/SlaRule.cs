namespace TicketAnnd.Domain.Entities;

public class SlaRule:BaseEntity
{   
    public Guid SlaPolicyId { get; set; }
    public virtual SlaPolicy SlaPolicy { get; set; }
    public int FirstResponseMinutes { get; set; }
    public int ResolutionMinutes  { get; set; }
    public string Name { get; set; }
}

    