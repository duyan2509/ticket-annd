namespace TicketAnnd.Domain.Entities;

public class TicketPause:BaseEntity
{
    public Guid TicketId { get; set; }
    public virtual Ticket Ticket { get; set; }  
    public DateTime PauseAt { get; set; } 
    public string Reason { get; set; }
    public virtual User PauseBy { get; set; }
    public virtual Guid PauseById { get; set; }
}