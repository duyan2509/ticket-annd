using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Domain.Entities;

public class Ticket:BaseEntity
{
    public virtual Company Company { get; set; }
    public Guid CompanyId { get; set; }
    public string TicketCode { get; set; }
    public Guid CustomerId { get; set; }
    public virtual User Customer { get; set; }
    public Guid? TeamId { get; set; }
    public virtual Team? Team { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Categrory category { get; set; }
    public string Subject { get; set; }
    public string? Body { get; set; }
    public Guid? TicketAssignId { get; set; }
    public virtual TicketAssign TicketAssign { get; set; }
    public TicketStatuses Status { get; set; }
    public virtual ICollection<TicketPick> TicketPicks { get; set; }
    public Guid SlaRuleId { get; set; }
    public virtual SlaRule SlaRule { get; set; }
    public DateTime FirstResponseDueAt {get;set;}
    public DateTime ResolutionDueAt {get;set;}
    public DateTime FirstResponseAt {get;set;}
    public DateTime ResolvedAt {get;set;}
    public bool IsResolutionBreached {get;set;}
    public bool IsResponseBreached {get;set;}

}


