using Microsoft.EntityFrameworkCore;
using EPS.Domain.Repositories;
using EPS.Persistence.Data;

namespace EPS.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EPSDbContext _context;
    private IMemberRepository? _memberRepository;
    private IEmployerRepository? _employerRepository;
    private IContributionRepository? _contributionRepository;
    private IBenefitEligibilityHistoryRepository? _benefitEligibilityHistoryRepository;
    private bool _disposed;

    public UnitOfWork(EPSDbContext context)
    {
        _context = context;
    }

    public IMemberRepository Members =>
        _memberRepository ??= new MemberRepository(_context);

    public IEmployerRepository Employers =>
        _employerRepository ??= new EmployerRepository(_context);

    public IContributionRepository Contributions =>
        _contributionRepository ??= new ContributionRepository(_context);

    public IBenefitEligibilityHistoryRepository BenefitEligibilityHistories =>
        _benefitEligibilityHistoryRepository ??= new BenefitEligibilityHistoryRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.Database.CommitTransactionAsync();
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
}