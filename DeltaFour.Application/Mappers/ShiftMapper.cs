using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class ShiftMapper
    {
        public static EmployeeShift FromCreateEmployeeDto(EmployeeShiftDto dto, Guid employeeId)
        {
            return new EmployeeShift()
            {
                EmployeeId = employeeId,
                ShiftId = dto.ShiftId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };
        }
    }
}
