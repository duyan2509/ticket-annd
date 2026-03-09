using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Entities;

public class Ticket:BaseEntity
{
    public virtual Company Company { get; set; }
    public Guid CompanyId { get; set; }
    public Guid RaiserId { get; set; }
    public virtual User Raiser { get; set; }
    public Guid? TeamId { get; set; }
    public virtual Team? Team { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Categrory Category { get; set; }
    public string Subject { get; set; }
    public string TicketCode { get; set; } = Guid.NewGuid().ToString("N")[..6].ToUpper();

    public string? Body { get; set; }
    public TicketStatuses Status { get; set; }
    public Guid SlaRuleId { get; set; }
    public virtual SlaRule SlaRule { get; set; }
    public int SlaFirstResponseMinutes {get;set;} 
    public int SlaResolutionMinutes {get;set;}
    public DateTime FirstResponseAt {get;set;} = DateTime.MinValue;
    public DateTime ResolvedAt {get;set;} = DateTime.MinValue;
    public bool IsResolutionBreached {get;set;} = false;
    public bool IsResponseBreached {get;set;} = false;

}


