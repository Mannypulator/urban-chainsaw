using EPS.Application.DTOs;
using EPS.Domain.Entities;

namespace EPS.Application.Mappings;

public static class EmployerMapping
{
    public static Employer MapToEntity(this CreateEmployerDto dto)
    {
        return new Employer
        {
            CompanyName = dto.Name,
            Address = dto.Address,
            ContactEmail = dto.Email,
            ContactPerson = dto.ContactPerson,
            ContactPhone = dto.Phone,
            IsActive = true,
            RegistrationDate = DateTime.Now,
            CreatedAt = DateTime.Now
        };
    }

    public static EmployerDto MapToDto(this Employer entity)
    {
        return new EmployerDto
        {
            Id = entity.Id,
            Name = entity.CompanyName,
            Address = entity.Address,
            ContactPerson = entity.ContactPerson,
            Email = entity.ContactEmail,
            Phone = entity.ContactPhone,
            CreatedAt = entity.CreatedAt,
            RegistrationNumber = entity.RegistrationNumber,
            LastModifiedAt = entity?.LastModifiedAt
        };
    }

    public static IReadOnlyList<EmployerDto> MapToDtoList(this IReadOnlyList<Employer> entities)
    {
        return entities.Select(entity => entity.MapToDto()).ToList();
    }
}