using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Entities;

public class TicketPick:BaseEntity
{
    public Guid TeamId { get; set; }
    public virtual Team Team { get; set; }
    public Guid TicketId { get; set; }
    public virtual Ticket Ticket { get; set; }
    public TicketPickStatuses Status { get; set; }
}

