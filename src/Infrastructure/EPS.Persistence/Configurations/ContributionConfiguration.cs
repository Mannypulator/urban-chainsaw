using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EPS.Domain.Entities;

namespace EPS.Persistence.Configurations;

public class ContributionConfiguration : IEntityTypeConfiguration<Contribution>
{
    public void Configure(EntityTypeBuilder<Contribution> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.ContributionDate)
            .IsRequired();

        builder.Property(c => c.Type)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.TransactionReference)
            .HasMaxLength(100);

        builder.Property(c => c.ValidationMessage)
            .HasMaxLength(500);

        builder.Property(c => c.InterestEarned)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(c => c.Member)
            .WithMany(m => m.Contributions)
            .HasForeignKey(c => c.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Create an index for efficient querying of monthly contributions
        builder.HasIndex(c => new { c.MemberId, c.ContributionDate, c.Type })
            .HasFilter("[IsDeleted] = 0");
    }
}