using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class ShiftMapper
    {
        ///<sumary>
        ///Map information from CreateUserDto to UserShift
        ///</sumary>
        public static UserShift FromCreateUserDto(UserShiftDto dto, Guid userId, Guid userAuthenticatedId)
        {
            return new UserShift()
            {
                UserId = userId,
                ShiftId = dto.ShiftId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate!,
                IsActive = dto.IsActive,
                CreatedBy = userAuthenticatedId,
            };
        }

        ///<sumary>
        ///Replace information from UserShift
        ///</sumary>
        public static void UpdateUserShift(UserShift shift, UserShiftDto dto, Guid userAuthenticatedId)
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
