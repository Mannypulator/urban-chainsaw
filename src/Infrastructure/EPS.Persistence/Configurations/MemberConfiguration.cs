using EPS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPS.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(m => m.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(m => m.PhoneNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne(m => m.Employer)
            .WithMany(e => e.Members)
            .HasForeignKey(m => m.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Contributions)
            .WithOne(c => c.Member)
            .HasForeignKey(c => c.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.EligibilityHistory)
            .WithOne(h => h.Member)
            .HasForeignKey(h => h.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}