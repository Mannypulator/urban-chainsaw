using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EPS.Domain.Entities;

namespace EPS.Persistence.Configurations;

public class BenefitEligibilityHistoryConfiguration : IEntityTypeConfiguration<BenefitEligibilityHistory>
{
    public void Configure(EntityTypeBuilder<BenefitEligibilityHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.IsEligible)
            .IsRequired();

        builder.Property(h => h.EvaluationDate)
            .IsRequired();

        builder.Property(h => h.Reason)
            .HasMaxLength(500);

        builder.Property(h => h.TotalContributions)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(h => h.Member)
            .WithMany(m => m.EligibilityHistory)
            .HasForeignKey(h => h.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Create an index for efficient querying of eligibility history
        builder.HasIndex(h => new { h.MemberId, h.EvaluationDate })
            .HasFilter("[IsDeleted] = 0");
    }
}