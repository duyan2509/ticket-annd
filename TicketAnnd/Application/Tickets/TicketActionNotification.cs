using MediatR;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Events;

namespace TicketAnnd.Application.Tickets;

public class TicketActionNotificationHandler : INotificationHandler<TicketActionNotification>
{
    private readonly ITicketLogRepository _repository;

    public TicketActionNotificationHandler(ITicketLogRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(TicketActionNotification notification, CancellationToken cancellationToken)
    {
        var doc = new TicketLogDocument
        {
            Id = notification.Id == Guid.Empty ? Guid.NewGuid() : notification.Id,
            TicketId = notification.TicketId,
            CompanyId = notification.CompanyId,
            Action = notification.Action,
            FromStatus = notification.FromStatus,
            ToStatus = notification.ToStatus,
            ActorId = notification.ActorId,
            ActorName = notification.ActorName,
            TargetId = notification.TargetId,
            TargetName = notification.TargetName,
            Note = notification.Note,
            Timestamp = notification.Timestamp == default ? DateTime.UtcNow : notification.Timestamp
        };

        await _repository.InsertAsync(doc, cancellationToken);
    }
}
