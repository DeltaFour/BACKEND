using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities
{
    public class UserAttendance : BaseEntity 
    {
        public Guid EmployeeId { get; set; }
        
        public DateTime PunchTime { get; set; }
        
        public PunchType PunchType { get; set; }
        
        public ShiftType ShiftType { get; set; }
        
        public Coordinates Coord { get; set; }
        
        public Boolean? IsLate { get; set; }
        
        public TimeOnly? TimeLate { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public Guid CreatedBy { get; set; }
         
        public Guid? UpdatedBy { get; set; }
        
        public User? Employee { get; set; }
    }
}