using Risk_Job.Domain.Interfaces;
using Risk_Job.Domain.Models;
using System.Collections.Concurrent;

namespace Risk_Job.Infrastructure.Repositories;

/// <summary>
/// Borrower risk repository implemented with in-memory storage.
/// </summary>
public class InMemoryBorrowerRiskRepository : IBorrowerRiskRepository
{
    private static readonly ConcurrentDictionary<Guid, BorrowerRisk> _storage = new();

    /// <inheritdoc/>
    public void AddOrUpdate(BorrowerRisk borrowerRisk)
    {
        _storage[borrowerRisk.BorrowerId] = borrowerRisk;
    }

    /// <inheritdoc/>
    public IEnumerable<BorrowerRisk> GetAll() => _storage.Values;
}
