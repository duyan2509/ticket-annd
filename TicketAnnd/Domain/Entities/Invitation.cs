using System;
using TicketAnnd.Application;
using TicketAnnd.Application.Invitation;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Events;

namespace TicketAnnd.Domain.Entities;

public class Invitation:BaseEntity
{
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; }
    public string Email { get; set; }
    public DateTime Expires { get; set; }
    public InviationStatuses Status { get; set; } = InviationStatuses.Pending;
    public AppRoles Role { get; set; } = AppRoles.Customer;
    public DateTime? ResponseAt { get; set; }
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }

    public bool IsExpires()
    {
        return DateTime.UtcNow > Expires;
    }
    public void Respond(Guid responderUserId, InviationStatuses newStatus)
    {
        if (IsExpires())
            throw new BusinessException("Invitation expired");

        if (Status != InviationStatuses.Pending)
            throw new BusinessException("Invitation already responded");

        Status = newStatus;
        ResponseAt = DateTime.UtcNow;

        if (UserId is null)
            UserId = responderUserId;

        if (Status == InviationStatuses.Accepted)
        {
            AddDomainEvent(new InvitationAcceptedEvent(
                Id,
                CompanyId,
                responderUserId,
                Role
            ));
        }
    
    }


    public void CreateNew(AppRoles role, Guid? userId, int expireDays, string companyName)
    {
        Expires = DateTime.UtcNow.AddDays(expireDays);
        Role = role;
        UserId = userId;
        Status = InviationStatuses.Pending;

        AddDomainEvent(new InvitationCreatedNotification(Email, companyName, role.ToString(), expireDays));
    }
    
    public void Update(Guid updaterUserId, string? email = null, DateTime? expires = null, AppRoles? role = null, int? expireDays = null, string? companyName = null)
    {
        if (IsExpires())
            throw new BusinessException("Invitation expired");

        if (Status != InviationStatuses.Pending)
            throw new BusinessException("Invitation already responded");

        var changed = false;
        if (email is not null && !string.Equals(Email, email, StringComparison.OrdinalIgnoreCase))
        {
            Email = email;
            changed = true;
        }

        if (expires.HasValue && Expires != expires.Value)
        {
            Expires = expires.Value;
            changed = true;
        }

        if (role.HasValue && Role != role.Value)
        {
            Role = role.Value;
            changed = true;
        }

        if (changed)
        {
            if (!string.IsNullOrWhiteSpace(companyName) && expireDays.HasValue)
            {
                AddDomainEvent(new InvitationCreatedNotification(
                    Email,
                    companyName,
                    (role ?? Role).ToString(),
                    expireDays.Value
                ));
            }
          
        }
    }

}
