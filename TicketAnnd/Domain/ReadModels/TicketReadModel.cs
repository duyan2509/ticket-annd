namespace TicketAnnd.Domain.ReadModels;

public class TicketReadModel
{
    public Guid Id { get; set; }
    public Guid RaiserId { get; set; }
    public string RaiserName { get; set; }
    public Guid? TeamId { get; set; }
    public string? TeamName { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Subject { get; set; }
    public string TicketCode { get; set; } 
    public string? Body { get; set; }
    public string Status { get; set; }
    public Guid SlaRuleId { get; set; }
    public string SlaRuleName { get; set; }
    public int SlaFirstResponseMinutes {get;set;} 
    public int SlaResolutionMinutes {get;set;}
    public DateTime? FirstResponseAt {get;set;} 
    public bool IsResolutionBreached {get;set;} = false;
    public bool IsResponseBreached {get;set;} = false;

}