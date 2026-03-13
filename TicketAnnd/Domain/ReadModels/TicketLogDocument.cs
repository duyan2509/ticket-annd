using System;

namespace TicketAnnd.Domain.ReadModels;

public class TicketLogDocument
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid CompanyId { get; set; }
    public string Action { get; set; }
    public string FromStatus { get; set; }
    public string ToStatus { get; set; }
    public Guid ActorId { get; set; }
    public string? Note { get; set; }
    public string? ActorName { get; set; }
    public Guid? TargetId { get; set; }
    public string? TargetName { get; set; }
    public DateTime Timestamp { get; set; }
}
