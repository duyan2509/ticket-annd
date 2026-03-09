using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAnnd.Domain.Entities;

namespace TicketAnnd.Infrastructure.Persistence.Configurations;

public class SlaRuleConfiguration : IEntityTypeConfiguration<SlaRule>
{
    public void Configure(EntityTypeBuilder<SlaRule> builder)
    {
        builder.ToTable("sla_rules");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x=>x.FirstResponseMinutes).IsRequired();
        builder.Property(x=>x.ResolutionMinutes).IsRequired();
        builder.HasOne(x => x.SlaPolicy)
            .WithMany(x=>x.SlaRules)
            .HasForeignKey(x => x.SlaPolicyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new
        {
            x.SlaPolicyId,
            x.Name
        }).IsUnique();
    }
}

