using System;
using System.Linq;
using TicketAnnd.Domain.Enums;
using NanoidDotNet;
using NpgsqlTypes;
using TicketAnnd.Domain.Events;
namespace TicketAnnd.Domain.Entities;

public class Ticket:BaseEntity
{
    public virtual Company Company { get; set; }
    public Guid CompanyId { get; set; }
    public Guid RaiserId { get; set; }
    public virtual User Raiser { get; set; }
    public Guid? TeamId { get; set; }
    public virtual Team? Team { get; set; }
    public Guid? AssigneeId { get; set; }
    public virtual User? Assignee { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Categrory Category { get; set; }
    public string Subject { get; set; }
    public string TicketCode { get; set; } = Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6);

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
    public virtual ICollection<TicketPause> TicketPauses { get; set; } = new List<TicketPause>();
    public NpgsqlTsVector SearchVector { get; set; }

    public void AddActionEvent(string action, Guid actorId, string? actorName = null, Guid? targetId = null, string? targetName = null, string? fromStatus = null, string? toStatus = null, string? note = null, DateTime? timestamp = null)
    {
        AddDomainEvent(new TicketActionNotification(
            Guid.NewGuid(),
            Id,
            CompanyId,
            action,
            fromStatus,
            toStatus,
            actorId,
            actorName,
            targetId,
            targetName,
            note,
            timestamp ?? DateTime.UtcNow
        ));
    }

    public void AssignTeam(Guid teamId, Guid actorId, string? actorName = null, string? targetName = null, string? note = null)
    {
        var from = Status.ToString();
        TeamId = teamId;
        Status = TicketStatuses.InProgress;
        AddActionEvent("AssignTeam", actorId, actorName: actorName, targetId: teamId, targetName: targetName, fromStatus: from, toStatus: Status.ToString(), note: note);
    }

    public void AssignMember(Guid assigneeId, Guid actorId, string? actorName = null, string? targetName = null, string? note = null)
    {
        var from = Status.ToString();
        AssigneeId = assigneeId;
        Status = TicketStatuses.InProgress;
        AddActionEvent("AssignMember", actorId, actorName: actorName, targetId: assigneeId, targetName: targetName, fromStatus: from, toStatus: Status.ToString(), note: note);
    }

    public void Continue(Guid actorId, string? actorName = null, string? note = null)
    {
        var from = Status.ToString();
            var lastPause = TicketPauses.OrderByDescending(p => p.PauseAt).FirstOrDefault();
            if (lastPause != null)
            {
                note = $"Continue from pause at {lastPause.PauseAt:u} with reason: {lastPause.Reason}";
            }

            Status = TicketStatuses.InProgress;
                AddActionEvent("Continue", actorId, actorName: actorName, fromStatus: from, toStatus: Status.ToString(), note: note);
    }

    public void Pause(Guid actorId, string pauseType, string reason, string? actorName = null)
    {
        var from = Status.ToString();
        TicketStatuses targetStatus;
        if (string.Equals(pauseType, nameof(TicketStatuses.WaitingCustomer), StringComparison.OrdinalIgnoreCase))
            targetStatus = TicketStatuses.WaitingCustomer;
        else if (string.Equals(pauseType, nameof(TicketStatuses.WaitingThirdParty), StringComparison.OrdinalIgnoreCase))
            targetStatus = TicketStatuses.WaitingThirdParty;
        else
            throw new InvalidOperationException("Invalid pause type");

        var pause = new TicketPause
        {
            TicketId = Id,
            PauseAt = DateTime.UtcNow,
            Reason = reason ?? string.Empty,
            PauseById = actorId
        };

        TicketPauses.Add(pause);
        Status = targetStatus;

        AddActionEvent("Pause", actorId, actorName: actorName, fromStatus: from, toStatus: Status.ToString(), note: reason);
    }

    public void Resolve(Guid actorId, string? actorName = null, string? note = null)
    {
        var from = Status.ToString();
        Status = TicketStatuses.Resolved;
        ResolvedAt = DateTime.UtcNow;
        AddActionEvent("Resolve", actorId, actorName: actorName, fromStatus: from, toStatus: Status.ToString(), note: note);
    }
}


