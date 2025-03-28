using System.Linq.Expressions;
using EPS.Domain.Common;
using EPS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPS.Persistence.Data;

public class EPSDbContext : DbContext
{
    public EPSDbContext(DbContextOptions<EPSDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members { get; set; }
    public DbSet<Employer> Employers { get; set; }
    public DbSet<Contribution> Contributions { get; set; }
    public DbSet<BenefitEligibilityHistory> BenefitEligibilityHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EPSDbContext).Assembly);

        // Global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var entityTypeBuilder = modelBuilder.Entity(entityType.ClrType);
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, "IsDeleted");
                var condition = Expression.Lambda(Expression.Not(property), parameter);
                entityTypeBuilder.HasQueryFilter(condition);
            }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "system"; // TODO: Get from current user
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = "system"; // TODO: Get from current user
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = "system"; // TODO: Get from current user
                    break;
            }

        return base.SaveChangesAsync(cancellationToken);
    }
}