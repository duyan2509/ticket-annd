namespace TicketAnnd.Domain.Entities;

public class Categrory:BaseEntity
{
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; }
    public string Name { get; set; }
}