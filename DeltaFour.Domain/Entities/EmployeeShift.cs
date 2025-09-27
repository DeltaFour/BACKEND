using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities
{
    public class EmployeeShift : BaseEntity
    {
        public Guid ShiftId { get; set; }
        
        public Guid EmployeeId { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public Employee? Employee { get; set; }
        
        public WorkShift? WorkShift { get; set; }
    }
}