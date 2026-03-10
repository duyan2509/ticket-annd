using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Infrastructure.Persistence.Configurations;

public class TicketPauseConfiguration: IEntityTypeConfiguration<TicketPause>
{
    public void Configure(EntityTypeBuilder<TicketPause> builder)
    {
        builder.ToTable("ticket_pauses");

        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.PauseAt)
            .IsRequired();

        builder.Property(tp => tp.Reason)
            .HasMaxLength(2000);

        builder.HasOne(tp=>tp.PauseBy)
            .WithMany()
            .HasForeignKey(tp => tp.PauseById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tp=>tp.Ticket)
            .WithMany(t=>t.TicketPauses)
            .HasForeignKey(tp => tp.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}