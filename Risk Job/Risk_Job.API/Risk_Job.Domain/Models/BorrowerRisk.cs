namespace Risk_Job.Domain.Models;

public class BorrowerRisk
{
    public Guid BorrowerId { get; set; }
    public int RiskScore { get; set; }
}
