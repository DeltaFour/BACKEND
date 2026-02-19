using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Mappers
{
    public static class UserMapper
    {
        ///<summary>
        ///Map information from CreateDto to User
        ///</summary>
        public static User FromCreateDto(UserCreateDto dto, Guid roleId, UserContext createdBy)
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

        ///<summary>
        ///Replace information from User
        ///</summary>
        public static void UpdateDataUserByUpdateDto(UserUpdateDto dto, User e, Guid userAuthenticatedId)
        {
            e.Name = dto.Name;
            e.Cellphone = dto.CellPhone;
            e.IsAllowedBypassCoord = dto.IsAllowedBypassCoord;
            e.UpdatedBy = userAuthenticatedId;
            e.UpdatedAt = DateTime.UtcNow;
        }
    }
}
