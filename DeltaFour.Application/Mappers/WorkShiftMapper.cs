using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class WorkShiftMapper
    {
        public static WorkShift CreateFromDto(WorkShiftDto dto, UserContext user)
        {
            return new WorkShift()
            {
                ShiftType = dto.ShiftType,
                StartTime = dto.StartDate,
                EndTime = dto.EndDate,
                ToleranceMinutes = dto.ToleranceMinutes,
                CompanyId = user.CompanyId,
                CreatedBy = user.Id,
            };
        }

        public static void UpdateDataWorkShift(WorkShift workShift, WorkShiftUpdateDto dto, UserContext user)
        {
            workShift.ShiftType = dto.ShiftType;
            workShift.StartTime = workShift.StartTime;
            workShift.EndTime = workShift.EndTime;
            workShift.ToleranceMinutes = dto.ToleranceMinutes;
            workShift.UpdatedBy = user.Id;
            workShift.UpdatedAt = DateTime.UtcNow;
        }
    }
}
