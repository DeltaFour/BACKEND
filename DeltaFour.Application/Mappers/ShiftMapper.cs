using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class ShiftMapper
    {
        public static EmployeeShift FromCreateEmployeeDto(EmployeeShiftDto dto, Guid employeeId, Guid userAuthenticatedId)
        {
            return new EmployeeShift()
            {
                EmployeeId = employeeId,
                ShiftId = dto.ShiftId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate!,
                CreatedBy = userAuthenticatedId,
            };
        }

        public static void UpdateEmployeeShift(EmployeeShift shift, EmployeeShiftDto dto, Guid userAuthenticatedId)
        {
            shift.StartDate = dto.StartDate;
            shift.EndDate = dto.EndDate;
            shift.ShiftId = dto.ShiftId;
            shift.UpdatedAt = DateTime.UtcNow;
            shift.UpdatedBy = userAuthenticatedId;
        }
    }
}
