using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities
{
    public class EmployeeAttendance : BaseEntity 
    {
        public Guid EmployeeId { get; set; }
        
        public DateTime PunchTime { get; set; }
        
        public PunchType PunchType { get; set; }
        
        public Coordinates Coord { get; set; }
        
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid CreatedBy { get; set; }
         
        public Guid? UpdatedBy { get; set; }
        
        public Employee? Employee { get; set; }
    }
}