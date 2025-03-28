using AutoMapper;
using EPS.Application.DTOs;
using EPS.Domain.Entities;

namespace EPS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Member, MemberDto>()
            .ForMember(d => d.EmployerName, opt => opt.MapFrom(s => s.Employer.CompanyName))
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.CalculateAge()));

        CreateMap<CreateMemberDto, Member>();

        CreateMap<Contribution, ContributionDto>()
            .ForMember(d => d.MemberName, opt => opt.MapFrom(s => $"{s.Member.FirstName} {s.Member.LastName}"));

        CreateMap<CreateContributionDto, Contribution>();
    }
}