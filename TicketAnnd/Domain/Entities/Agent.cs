namespace TicketAnnd.Domain.Entities;

public class Agent:BaseEntity
{
    public Guid Name { get; set; }
    public virtual Company Company { get; set; }
    public Guid CompanyId { get; set; }
    public ICollection<UserAgent> UserAgents { get; set; }
}