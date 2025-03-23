using EPS.Application.DTOs;
using EPS.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMemberService memberService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> CreateMember([FromBody] CreateMemberDto createDto)
    {
        try
        {
            var member = await _memberService.CreateMemberAsync(createDto);
            return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating member");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetMember(Guid id)
    {
        try
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null)
                return NotFound();

            return Ok(member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving member {MemberId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetAllMembers()
    {
        try
        {
            var members = await _memberService.GetAllMembersAsync();
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all members");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("employer/{employerId}")]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetMembersByEmployer(Guid employerId)
    {
        try
        {
            var members = await _memberService.GetMembersByEmployerAsync(employerId);
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving members for employer {EmployerId}", employerId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMember(Guid id, [FromBody] UpdateMemberDto updateDto)
    {
        try
        {
            await _memberService.UpdateMemberAsync(id, updateDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating member {MemberId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(Guid id)
    {
        try
        {
            await _memberService.DeleteMemberAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting member {MemberId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/contributions/total")]
    public async Task<ActionResult<decimal>> GetTotalContributions(Guid id)
    {
        try
        {
            var total = await _memberService.GetTotalContributionsAsync(id);
            return Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total contributions for member {MemberId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/benefits/eligibility")]
    public async Task<ActionResult<bool>> CheckBenefitEligibility(Guid id)
    {
        try
        {
            var isEligible = await _memberService.IsMemberEligibleForBenefitsAsync(id);
            return Ok(isEligible);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking benefit eligibility for member {MemberId}", id);
            return BadRequest(ex.Message);
        }
    }
}