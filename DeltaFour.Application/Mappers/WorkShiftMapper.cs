using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;

namespace DeltaFour.Application.Mappers
{
    public static class WorkShiftMapper
    {
        ///<sumary>
        ///Map information from Dto to WorkShift
        ///</sumary>
        public static WorkShift CreateFromDto(WorkShiftDto dto, UserContext user)
        {
            return new WorkShift()
            {
                ShiftType = dto.ShiftType,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                ToleranceMinutes = dto.ToleranceMinutes,
                CompanyId = user.CompanyId,
                CreatedBy = user.Id,
            };
        }

        ///<sumary>
        ///Replace information from WorkShift
        ///</sumary>
        public static void UpdateDataWorkShift(WorkShift workShift, WorkShiftUpdateDto dto, UserContext user)
        {
            workShift.ShiftType = dto.ShiftType;
            workShift.StartTime = workShift.StartTime;
            workShift.EndTime = workShift.EndTime;
            workShift.ToleranceMinutes = dto.ToleranceMinutes;
            workShift.UpdatedBy = user.Id;
            workShift.UpdatedAt = DateTime.UtcNow;
        }

        ///<sumary>
        ///Map information from WorkShift to WorkShiftPunchDto
        ///</sumary>
        public static WorkShiftPunchDto FromWorkShift(WorkShift workShift)
        {
            return new WorkShiftPunchDto()
            {
                StartTime = workShift.StartTime,
                EndTime = workShift.EndTime,
                ToleranceMinutes = workShift.ToleranceMinutes,
            };
        }
    }
}
