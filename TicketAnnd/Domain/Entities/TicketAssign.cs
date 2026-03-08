namespace TicketAnnd.Domain.Entities;

public class TicketAssign:BaseEntity
{
    public Guid TeamId { get; set; }
    public virtual Team Team { get; set; }
    public Guid TicketId { get; set; }
    public virtual Ticket Ticket { get; set; }
}