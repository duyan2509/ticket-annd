using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Infrastructure.Persistence.Configurations;

public class TicketAssignConfiguration : IEntityTypeConfiguration<TicketAssign>
{
    public void Configure(EntityTypeBuilder<TicketAssign> builder)
    {
        builder.ToTable("ticket_assigns");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Team)
            .WithMany()
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Ticket)
            .WithMany()
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
