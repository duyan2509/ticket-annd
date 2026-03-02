namespace TicketAnnd.Domain.Entities;

public class UserAgent : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public Guid AgentId { get; set; }
    public virtual Agent Agent { get; set; }
}