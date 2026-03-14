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
        // Migrated ticket action persistence to the outbox pattern.
        // Background processor will move outbox payloads to MongoDB.
        await Task.CompletedTask;
    }
}
