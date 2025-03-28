using EPS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPS.Persistence.Configurations;

public class EmployerConfiguration : IEntityTypeConfiguration<Employer>
{
    public void Configure(EntityTypeBuilder<Employer> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ContactEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(e => e.RegistrationNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.ContactEmail)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasMany(e => e.Members)
            .WithOne(m => m.Employer)
            .HasForeignKey(m => m.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}