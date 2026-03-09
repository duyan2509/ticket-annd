namespace TicketAnnd.Domain.Entities;

public class TeamMember : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public Guid TeamId { get; set; }
    public virtual Team Team { get; set; }
    public bool IsLeader { get; set; } = false;
}
