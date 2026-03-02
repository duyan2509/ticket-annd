using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Entities;

public class TicketPick:BaseEntity
{
    public Guid AgentId { get; set; }
    public virtual Agent Agent { get; set; }
    public Guid TicketId { get; set; }
    public virtual Ticket Ticket { get; set; }
    public TicketPickStatuses Status { get; set; }
}

