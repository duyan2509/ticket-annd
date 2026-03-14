using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Events;

namespace TicketAnnd.Infrastructure.Persistence;

public class TicketAnndDbContext : DbContext
{
    public TicketAnndDbContext(DbContextOptions<TicketAnndDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }
    private readonly IMediator _mediator;

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Categrory> Categories => Set<Categrory>();
    public DbSet<SlaPolicy> SlaPolicies => Set<SlaPolicy>();
    public DbSet<SlaRule> SlaRules => Set<SlaRule>();
    public DbSet<UserCompanyRole> UserCompanyRoles => Set<UserCompanyRole>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<TicketPause> TicketPauses => Set<TicketPause>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketAnndDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        if (domainEvents.Any())
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            options.Converters.Add(new JsonStringEnumConverter());

            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent is not TicketActionNotification) continue;
                else
                {
                    var payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), options);

                    if (payload == "{}")
                    {
                        try
                        {
                            Console.WriteLine($"[OutboxDebug] Serialized payload for {domainEvent.GetType().FullName} was empty.");
                            foreach (var prop in domainEvent.GetType().GetProperties())
                            {
                                var val = prop.GetValue(domainEvent);
                                Console.WriteLine($"[OutboxDebug] {prop.Name} = {val}");
                            }
                        }
                        catch { /* swallow debug errors */ }
                    }

                    var outbox = new OutboxMessage
                    {
                        Id = Guid.NewGuid(),
                        EventType = domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                        Payload = payload,
                        OccurredAt = DateTime.UtcNow,
                        Status = "Pending"
                    };
                    OutboxMessages.Add(outbox);
                }
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {

            if (domainEvent is TicketActionNotification)
                continue;

            await _mediator.Publish(domainEvent, cancellationToken);
        }
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return result;
    }

}
