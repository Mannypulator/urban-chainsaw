using EPS.API.ActionFilters;
using EPS.Application.DTOs;
using EPS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployersController : ControllerBase
{
    private readonly IEmployerService _employerService;
    private readonly ILogger<EmployersController> _logger;

    public EmployersController(IEmployerService employerService, ILogger<EmployersController> logger)
    {
        _employerService = employerService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployerDto), StatusCodes.Status200OK)]
    // [ServiceFilter(typeof(ValidationModelFilterAttribute<CreateEmployerDto>))]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployer([FromBody] CreateEmployerDto request)
    {
        var employer = await _employerService.CreateEmployerAsync(request);
        return CreatedAtAction(nameof(GetEmployer), new { id = employer.Id }, employer);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployer(Guid id)
    {
        var employer = await _employerService.GetEmployerByIdAsync(id);
        if (employer == null) return NotFound();

        return Ok(employer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmployers()
    {
        var employers = await _employerService.GetAllEmployersAsync();
        return Ok(employers);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveEmployers()
    {
        var employers = await _employerService.GetActiveEmployersAsync();
        return Ok(employers);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(IReadOnlyList<ContributionDto>), StatusCodes.Status200OK)]
    [ServiceFilter(typeof(ValidationModelFilterAttribute<UpdateEmployerDto>))]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateEmployer(Guid id, [FromBody] UpdateEmployerDto request)
    {
        await _employerService.UpdateEmployerAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateEmployer(Guid id)
    {
        await _employerService.DeactivateEmployerAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/members")]
    public async Task<IActionResult> GetEmployerMembers(Guid id)
    {
        var members = await _employerService.GetEmployerMembersAsync(id);
        return Ok(members);
    }

    [HttpGet("{id}/contributions/total")]
    public async Task<IActionResult> GetTotalContributions(
        Guid id,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var total = await _employerService.GetTotalContributionsAsync(id, startDate, endDate);
        return Ok(total);
    }

    [HttpGet("{id}/validate")]
    public async Task<IActionResult> ValidateEmployer(Guid id)
    {
        var isValid = await _employerService.ValidateEmployerAsync(id);
        return Ok(isValid);
    }
}