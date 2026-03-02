using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Infrastructure.Persistence.Configurations;

public class UserAgentConfiguration : IEntityTypeConfiguration<UserAgent>
{
    public void Configure(EntityTypeBuilder<UserAgent> builder)
    {
        builder.ToTable("user_agents");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.AgentId })
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserAgents)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Agent)
            .WithMany(a => a.UserAgents)
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

