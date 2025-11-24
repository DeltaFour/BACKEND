using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities
{
    public class WorkShift : BaseEntity
    {
        public ShiftType ShiftType { get; set; }
        
        public TimeOnly StartTime { get; set; }
        
        public TimeOnly EndTime { get; set; }
        
        public int ToleranceMinutes { get; set; }
        
        public Guid CompanyId { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public List<UserShift>? EmployeeShifts { get; set; }
        
        public Company Company { get; set; }
    }
}