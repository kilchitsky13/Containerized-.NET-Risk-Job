using Microsoft.AspNetCore.Mvc;
using Risk_Job.Domain.Interfaces;

namespace Risk_Job.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RiskJobController : ControllerBase
{
    private readonly ILogger<RiskJobController> _logger;
    private readonly IBorrowerRiskRepository _borrowerRiskRepository;

    public RiskJobController(
        IBorrowerRiskRepository borrowerRiskRepository,
        ILogger<RiskJobController> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(borrowerRiskRepository);

        _logger = logger;
        _borrowerRiskRepository = borrowerRiskRepository;
    }

    [HttpGet("run-risk-job")]
    public IActionResult Get()
    {
        _logger.LogInformation("Risk job started.");

        // Simulate risk job processing
        Thread.Sleep(1000); // Simulate some processing time

        var risks = _borrowerRiskRepository.GetAll();
        _logger.LogInformation("Risk job completed.");

        if (!risks.Any())
        {
            var message = "No borrower risk records found.";
            _logger.LogWarning(message);
            return NotFound(message);
        }

        _logger.LogInformation("Retrieved {Count} borrower risk records.", risks.Count());

        return Ok(risks);
    }

    [HttpPost("init-risk")]
    public IActionResult AddRisk()
    {
        _logger.LogInformation("Initializing borrower risk record.");

        // Simulate adding some borrower risk records
        var borrowerRisk = new Domain.Models.BorrowerRisk
        {
            BorrowerId = Guid.NewGuid(),
            RiskScore = Random.Shared.Next(1, 100)
        };

        _logger.LogInformation(
            "Adding borrower risk: {BorrowerId} with Risk Score: {RiskScore}", borrowerRisk.BorrowerId, borrowerRisk.RiskScore);
        _borrowerRiskRepository.AddOrUpdate(borrowerRisk);

        return Created();
    }
}
