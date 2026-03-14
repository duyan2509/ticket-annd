using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TicketAnnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Events;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Infrastructure.Persistence;

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Events;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Infrastructure.Persistence;

namespace TicketAnnd.Infrastructure.Services;

public class OutboxProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ProcessPendingAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TicketAnndDbContext>();
        var ticketLogRepo = scope.ServiceProvider.GetService<ITicketLogRepository>();

        var pending = await db.OutboxMessages
            .Where(o => o.Status == "Pending")
            .OrderBy(o => o.OccurredAt)
            .Take(50)
            .ToListAsync();

        if (!pending.Any())
            return;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        options.Converters.Add(new JsonStringEnumConverter());

        foreach (var outbox in pending)
        {
            try
            {
                if (outbox.EventType == typeof(TicketActionNotification).FullName)
                {
                    if (ticketLogRepo == null)
                    {
                        _logger.LogWarning("No ITicketLogRepository registered; skipping outbox item {OutboxId}", outbox.Id);
                        continue;
                    }

                    var evt = JsonSerializer.Deserialize<TicketActionNotification>(outbox.Payload, options);
                    if (evt == null)
                        throw new InvalidOperationException("Failed to deserialize outbox payload");

                    var doc = new TicketLogDocument
                    {
                        Id = evt.Id == Guid.Empty ? Guid.NewGuid() : evt.Id,
                        TicketId = evt.TicketId,
                        CompanyId = evt.CompanyId,
                        Action = evt.Action,
                        FromStatus = evt.FromStatus,
                        ToStatus = evt.ToStatus,
                        ActorId = evt.ActorId,
                        ActorName = evt.ActorName,
                        TargetId = evt.TargetId,
                        TargetName = evt.TargetName,
                        Note = evt.Note,
                        Timestamp = evt.Timestamp == default ? DateTime.UtcNow : evt.Timestamp
                    };

                    await ticketLogRepo.InsertAsync(doc, default);
                }

                outbox.ProcessedAt = DateTime.UtcNow;
                outbox.Status = "Processed";
                db.OutboxMessages.Update(outbox);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox message {OutboxId}", outbox.Id);
                outbox.Status = "Pending";
                outbox.OccurredAt = DateTime.UtcNow;
                db.OutboxMessages.Update(outbox);
                await db.SaveChangesAsync();
            }
        }
    }
}
        
