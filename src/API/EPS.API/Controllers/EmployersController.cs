using EPS.Application.DTOs;
using EPS.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    public async Task<ActionResult<EmployerDto>> CreateEmployer([FromBody] CreateEmployerDto createDto)
    {
        try
        {
            var employer = await _employerService.CreateEmployerAsync(createDto);
            return CreatedAtAction(nameof(GetEmployer), new { id = employer.Id }, employer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employer");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployerDto>> GetEmployer(Guid id)
    {
        try
        {
            var employer = await _employerService.GetEmployerByIdAsync(id);
            if (employer == null)
                return NotFound();

            return Ok(employer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employer {EmployerId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployerDto>>> GetAllEmployers()
    {
        try
        {
            var employers = await _employerService.GetAllEmployersAsync();
            return Ok(employers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all employers");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<EmployerDto>>> GetActiveEmployers()
    {
        try
        {
            var employers = await _employerService.GetActiveEmployersAsync();
            return Ok(employers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active employers");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployer(Guid id, [FromBody] UpdateEmployerDto updateDto)
    {
        try
        {
            await _employerService.UpdateEmployerAsync(id, updateDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employer {EmployerId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateEmployer(Guid id)
    {
        try
        {
            await _employerService.DeactivateEmployerAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating employer {EmployerId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/members")]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetEmployerMembers(Guid id)
    {
        try
        {
            var members = await _employerService.GetEmployerMembersAsync(id);
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving members for employer {EmployerId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/contributions/total")]
    public async Task<ActionResult<decimal>> GetTotalContributions(
        Guid id,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var total = await _employerService.GetTotalContributionsAsync(id, startDate, endDate);
            return Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total contributions for employer {EmployerId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/validate")]
    public async Task<ActionResult<bool>> ValidateEmployer(Guid id)
    {
        try
        {
            var isValid = await _employerService.ValidateEmployerAsync(id);
            return Ok(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating employer {EmployerId}", id);
            return BadRequest(ex.Message);
        }
    }
}