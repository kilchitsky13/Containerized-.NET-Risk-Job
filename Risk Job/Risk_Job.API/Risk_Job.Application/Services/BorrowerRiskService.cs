using Risk_Job.Application.Interfaces;

namespace Risk_Job.Application.Services;

public class BorrowerRiskService: IBorrowerRiskService
{
    // Simulate pulling borrower risk data
    var fakeData = new List<BorrowerRisk>
    {
        new BorrowerRisk { BorrowerId = Guid.NewGuid(), RiskScore = Random.Shared.Next(300, 850) },
        new BorrowerRisk { BorrowerId = Guid.NewGuid(), RiskScore = Random.Shared.Next(300, 850) },
        new BorrowerRisk { BorrowerId = Guid.NewGuid(), RiskScore = Random.Shared.Next(300, 850) }
    };

}
