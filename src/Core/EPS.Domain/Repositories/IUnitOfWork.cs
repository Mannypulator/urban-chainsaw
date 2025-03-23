namespace EPS.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IMemberRepository Members { get; }
    IEmployerRepository Employers { get; }
    IContributionRepository Contributions { get; }
    IBenefitEligibilityHistoryRepository BenefitEligibilityHistories { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}