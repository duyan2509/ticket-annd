using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketAnnd.Domain.Entities;

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


        var result = await base.SaveChangesAsync(cancellationToken);
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

}
