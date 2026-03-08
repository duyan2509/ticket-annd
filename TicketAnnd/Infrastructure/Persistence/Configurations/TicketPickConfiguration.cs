using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Infrastructure.Persistence.Configurations;

public class TicketPickConfiguration : IEntityTypeConfiguration<TicketPick>
{
    public void Configure(EntityTypeBuilder<TicketPick> builder)
    {
        builder.ToTable("ticket_picks");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Team)
            .WithMany()
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Ticket)
            .WithMany(t => t.TicketPicks)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

