using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class UserResponseAttendanceDto
    {
        public Guid Id { get; set; }
        
        public DateTime? PunchTime { get; set; }
        
        public PunchType? PunchType { get; set; }
    }
}
