using EPS.Application.DTOs;

namespace EPS.Application.Services;

public interface IMemberService
{
    Task<MemberDto> CreateMemberAsync(CreateMemberDto createMemberDto);
    Task<MemberDto> UpdateMemberAsync(Guid id, UpdateMemberDto updateMemberDto);
    Task<MemberDto> GetMemberByIdAsync(Guid id);
    Task<IReadOnlyList<MemberDto>> GetAllMembersAsync();
    Task<IReadOnlyList<MemberDto>> GetMembersByEmployerAsync(Guid employerId);
    Task DeleteMemberAsync(Guid id);
    Task<decimal> GetTotalContributionsAsync(Guid memberId);
    Task<bool> IsMemberEligibleForBenefitsAsync(Guid memberId);
    Task UpdateBenefitEligibilityAsync(Guid memberId);
}