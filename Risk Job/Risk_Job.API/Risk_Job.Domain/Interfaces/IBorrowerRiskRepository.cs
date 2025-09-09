using Risk_Job.Domain.Models;

namespace Risk_Job.Domain.Interfaces;

public interface IBorrowerRiskRepository
{
    /// <summary>
    /// Add or update a borrower's risk information.
    /// </summary>
    /// <param name="borrowerRisk"></param>
    void AddOrUpdate(BorrowerRisk borrowerRisk);

    /// <summary>
    /// Retrieve all borrower risk records.
    /// </summary>
    /// <returns></returns>
    IEnumerable<BorrowerRisk> GetAll();
}
