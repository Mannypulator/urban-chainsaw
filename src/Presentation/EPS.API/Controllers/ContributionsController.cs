using EPS.API.ActionFilters;
using EPS.Application.DTOs;
using EPS.Application.Services;
using EPS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

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
    [ServiceFilter(typeof(ValidationModelFilterAttribute<CreateContributionDto>))]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(typeof(ContributionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContribution([FromBody] CreateContributionDto request)
    {
        var contribution = await _contributionService.CreateContributionAsync(request);
        return CreatedAtAction(nameof(GetContribution), new { id = contribution.Id }, contribution);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContributionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContribution(Guid id)
    {
        var contribution = await _contributionService.GetContributionByIdAsync(id);
        if (contribution == null) return NotFound();

        return Ok(contribution);
    }

    [HttpGet("member/{memberId}")]
    [ProducesResponseType(typeof(IReadOnlyList<ContributionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMemberContributions(Guid memberId)
    {
        var contributions = await _contributionService.GetMemberContributionsAsync(memberId);
        return Ok(contributions);
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IReadOnlyList<ContributionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContributionsByStatus(ContributionStatus status)
    {
        var contributions = await _contributionService.GetContributionsByStatusAsync(status);
        return Ok(contributions);
    }

    [HttpPost("{id}/validate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateContribution(Guid id)
    {
        await _contributionService.ValidateContributionAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/process")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessContribution(Guid id)
    {
        await _contributionService.ProcessContributionAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/calculate-interest")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CalculateInterest(Guid id)
    {
        await _contributionService.CalculateInterestAsync(id);
        return NoContent();
    }

    [HttpGet("member/{memberId}/total")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTotalContributionsForPeriod(
        Guid memberId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var total = await _contributionService.GetTotalContributionsForPeriodAsync(memberId, startDate, endDate);
        return Ok(total);
    }

    [HttpGet("member/{memberId}/validate-monthly")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateMonthlyContribution(
        Guid memberId,
        [FromQuery] DateTime contributionDate)
    {
        var hasContribution = await _contributionService.ValidateMonthlyContributionAsync(memberId, contributionDate);
        return Ok(hasContribution);
    }
}