using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Mappers
{
    public static class AuthMapper
    {
        ///<summary>
        ///Map information from user to TreatedUserInformation
        ///</summary>
        public static TreatedUserInformationDto FromUserToTreatedUserInfo(User user)
        {
            return new TreatedUserInformationDto()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleName = user.Role?.Name,
                RoleId = user.RoleId,
                IsAllowedBypassCoord = user.IsAllowedBypassCoord,
                CompanyId = user.CompanyId,
            };
        }
        
        ///<summary>
        ///Map information from TreatedUserInformation to the UserContext
        ///</summary>
        public static UserContext UserContext(TreatedUserInformationDto user)
        {
            return new UserContext()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                IsAllowedBypassCoord = user.IsAllowedBypassCoord,
                CompanyId = user.CompanyId,
            };
        }

        ///<summary>
        ///Map information from user to UserInfoLoginDto
        ///</summary>
        public static UserInfoLoginDto MapUserToUserInfoLoginDto(TreatedUserInformationDto dto)
        {
            UserInfoLoginDto teste = new UserInfoLoginDto()
            {
                Name = dto.Name,
                CompanyName = dto.CompanyName,
                Role = dto.RoleName,
                StartDate = dto.UserShift?.StartDate!,
                ShiftType = dto.UserShift?.ShiftType!,
                StartTime = dto.UserShift?.StartTime!,
                EndTime = dto.UserShift?.EndTime!,
                LastPunchType = dto.LastPunchType!,
                LastUserAttendances = dto.LastsUserAttendances!
            };
            return teste;
        }
    }
}