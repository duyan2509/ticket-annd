namespace TicketAnnd.Domain.Events;
public record TicketActionNotification(Guid Id, Guid TicketId, Guid CompanyId, string Action, string? FromStatus, string? ToStatus, Guid ActorId, string? Note, DateTime Timestamp): IDomainEvent;
