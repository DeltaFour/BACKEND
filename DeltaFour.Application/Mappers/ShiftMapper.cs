using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class ShiftMapper
    {
        public static UserShift FromCreateEmployeeDto(EmployeeShiftDto dto, Guid employeeId, Guid userAuthenticatedId)
        {
            return new UserShift()
            {
                EmployeeId = employeeId,
                ShiftId = dto.ShiftId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate!,
                IsActive = dto.IsActive,
                CreatedBy = userAuthenticatedId,
            };
        }

        public static void UpdateEmployeeShift(UserShift shift, EmployeeShiftDto dto, Guid userAuthenticatedId)
        {
            shift.StartDate = dto.StartDate;
            shift.EndDate = dto.EndDate;
            shift.ShiftId = dto.ShiftId;
            shift.IsActive = dto.IsActive;
            shift.UpdatedAt = DateTime.UtcNow;
            shift.UpdatedBy = userAuthenticatedId;
        }
    }
}
