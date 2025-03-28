using EPS.API.ActionFilters;
using EPS.Application.DTOs;
using EPS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly ILogger<MembersController> _logger;
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ServiceFilter(typeof(ValidationModelFilterAttribute<CreateMemberDto>))]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateMember([FromBody] CreateMemberDto request)
    {
        var member = await _memberService.CreateMemberAsync(request);
        return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMember(Guid id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null) return NotFound();

        return Ok(member);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MemberDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMembers()
    {
        var members = await _memberService.GetAllMembersAsync();
        return Ok(members);
    }

    [HttpGet("employer/{employerId}")]
    [ProducesResponseType(typeof(IReadOnlyList<MemberDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMembersByEmployer(Guid employerId)
    {
        var members = await _memberService.GetMembersByEmployerAsync(employerId);
        return Ok(members);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMember(Guid id, [FromBody] UpdateMemberDto request)
    {
        await _memberService.UpdateMemberAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(Guid id)
    {
        await _memberService.DeleteMemberAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/contributions/total")]
    public async Task<IActionResult> GetTotalContributions(Guid id)
    {
        var total = await _memberService.GetTotalContributionsAsync(id);
        return Ok(total);
    }

    [HttpGet("{id}/benefits/eligibility")]
    public async Task<IActionResult> CheckBenefitEligibility(Guid id)
    {
        var isEligible = await _memberService.IsMemberEligibleForBenefitsAsync(id);
        return Ok(isEligible);
    }
}