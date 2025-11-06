using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities
{
    public class WorkShift : BaseEntity
    {
        public ShiftType ShiftType { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public int ToleranceMinutes { get; set; }
        
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid? UpdatedBy { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public List<EmployeeShift>? EmployeeShifts { get; set; }
    }
}