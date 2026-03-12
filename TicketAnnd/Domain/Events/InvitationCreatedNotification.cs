using System;

namespace TicketAnnd.Domain.Events;

public record InvitationCreatedNotification(string To, string CompanyName, string Role, int ExpireDays) : TicketAnnd.Domain.Events.IDomainEvent;


