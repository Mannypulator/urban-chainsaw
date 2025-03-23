using EPS.Application.DTOs;
using EPS.Application.Services;
using EPS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContributionsController : ControllerBase
{
    private readonly IContributionService _contributionService;
    private readonly ILogger<ContributionsController> _logger;

    public ContributionsController(IContributionService contributionService, ILogger<ContributionsController> logger)
    {
        _contributionService = contributionService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContributionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContributionDto>> CreateContribution([FromBody] CreateContributionDto createDto)
    {
        try
        {
            var contribution = await _contributionService.CreateContributionAsync(createDto);
            return CreatedAtAction(nameof(GetContribution), new { id = contribution.Id }, contribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contribution");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContributionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContributionDto>> GetContribution(Guid id)
    {
        try
        {
            var contribution = await _contributionService.GetContributionByIdAsync(id);
            if (contribution == null)
                return NotFound();

            return Ok(contribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contribution {ContributionId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("member/{memberId}")]
    [ProducesResponseType(typeof(IReadOnlyList<ContributionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContributionDto>>> GetMemberContributions(Guid memberId)
    {
        try
        {
            var contributions = await _contributionService.GetMemberContributionsAsync(memberId);
            return Ok(contributions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contributions for member {MemberId}", memberId);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IReadOnlyList<ContributionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContributionDto>>> GetContributionsByStatus(ContributionStatus status)
    {
        try
        {
            var contributions = await _contributionService.GetContributionsByStatusAsync(status);
            return Ok(contributions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contributions with status {Status}", status);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/validate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateContribution(Guid id)
    {
        try
        {
            await _contributionService.ValidateContributionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating contribution {ContributionId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/process")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessContribution(Guid id)
    {
        try
        {
            await _contributionService.ProcessContributionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing contribution {ContributionId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/calculate-interest")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CalculateInterest(Guid id)
    {
        try
        {
            await _contributionService.CalculateInterestAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating interest for contribution {ContributionId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("member/{memberId}/total")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<decimal>> GetTotalContributionsForPeriod(
        Guid memberId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var total = await _contributionService.GetTotalContributionsForPeriodAsync(memberId, startDate, endDate);
            return Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total contributions for member {MemberId}", memberId);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("member/{memberId}/validate-monthly")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ValidateMonthlyContribution(
        Guid memberId,
        [FromQuery] DateTime contributionDate)
    {
        try
        {
            var hasContribution = await _contributionService.ValidateMonthlyContributionAsync(memberId, contributionDate);
            return Ok(hasContribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating monthly contribution for member {MemberId}", memberId);
            return BadRequest(ex.Message);
        }
    }
}