using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class EmployeeMapper
    {
        public static Employee FromCreateDto(EmployeeCreateDto dto, Guid roleId, Employee createdBy)
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

        public static void UpdateDataEmployeeByUpdateDto(EmployeeUpdateDto dto, Employee e, Guid userAuthenticatedId)
        {
            e.Name = dto.Name;
            e.RoleId = dto.RoleId;
            e.Cellphone = dto.CellPhone;
            e.IsAllowedBypassCoord = dto.IsAllowedBypassCoord;
            e.UpdatedBy = userAuthenticatedId;
            e.UpdatedAt = DateTime.UtcNow;
        }
    }
}
