using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TicketCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Body)
            .HasMaxLength(4000);

        builder.HasIndex(t => new { t.CompanyId, t.TicketCode })
            .IsUnique();

        builder.HasOne(t => t.Company)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Customer)
            .WithMany()
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Agent)
            .WithMany()
            .HasForeignKey(t => t.AgentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.SlaRule)
            .WithMany()
            .HasForeignKey(t => t.SlaRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TicketAssign)
            .WithOne()
            .HasForeignKey<Ticket>(t => t.TicketAssignId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

