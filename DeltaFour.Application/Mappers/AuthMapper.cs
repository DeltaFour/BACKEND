using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Mappers
{
    public static class AuthMapper
    {
        public static TreatedUserInformationDto FromEmployeeToTreatedUserInfo(Employee employee)
        {
            return new TreatedUserInformationDto()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                RoleId = employee.RoleId,
                IsAllowedBypassCoord = employee.IsAllowedBypassCoord,
                CompanyId = employee.CompanyId,
            };
        }
        
        public static UserContext UserContext (TreatedUserInformationDto employee)
        {
            return new UserContext()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                RoleId = employee.RoleId,
                IsAllowedBypassCoord = employee.IsAllowedBypassCoord,
                CompanyId = employee.CompanyId,
            };
        }

        public static UserInfoLoginDto MapUserToUserInfoLoginDto(TreatedUserInformationDto dto)
        {
            UserInfoLoginDto teste = new UserInfoLoginDto()
            {
                Name = dto.Name,
                CompanyName = dto.CompanyName,
                StartDate = dto.EmployeeShift?.StartDate!,
                ShiftType = dto.EmployeeShift?.ShiftType!,
                StartTime = dto.EmployeeShift?.StartTime!,
                EndTime = dto.EmployeeShift?.EndTime!,
                LastPunchType = dto.LastPunchType!,
                LastsEmployeeAttendances = dto.LastsEmployeeAttendances!
            };
            return teste;
        }
    }
}