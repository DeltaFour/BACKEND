using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class EmployeeMapper
    {
        public static Employee fromCreateDto(EmployeeCreateDto dto, Guid roleId, Employee createdBy)
        {
            return new Employee()
            {
                CompanyId = createdBy.CompanyId,
                RoleId = roleId,
                Name = dto.Name,
                Password = dto.Password,
                Cellphone = dto.CellPhone!,
                Email = dto.Email,
                IsActive = true,
                IsConfirmed = true,
                IsAllowedBypassCoord = dto.IsAllowedBypassCoord,
                CreatedBy = createdBy.Id,
            };
        }

        public static EmployeeResponseDto fromListEmployee(Employee e)
        {
            return new EmployeeResponseDto(e.Id, e.Name, e.Email, e.Role.Name, e.IsActive, e.IsAllowedBypassCoord, 
                e.LastLogin);
        }
    }
}
