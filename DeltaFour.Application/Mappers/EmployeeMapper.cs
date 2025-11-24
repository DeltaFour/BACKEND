using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Mappers
{
    public static class EmployeeMapper
    {
        public static User FromCreateDto(EmployeeCreateDto dto, Guid roleId, UserContext createdBy)
        {
            return new User()
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

        public static void UpdateDataEmployeeByUpdateDto(EmployeeUpdateDto dto, User e, Guid userAuthenticatedId)
        {
            e.Name = dto.Name;
            e.Cellphone = dto.CellPhone;
            e.IsAllowedBypassCoord = dto.IsAllowedBypassCoord;
            e.UpdatedBy = userAuthenticatedId;
            e.UpdatedAt = DateTime.UtcNow;
        }
    }
}
