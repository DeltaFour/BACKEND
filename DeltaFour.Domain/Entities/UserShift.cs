using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities
{
    public class UserShift : BaseEntity
    {
        public Guid ShiftId { get; set; }
        
        public Guid UserId { get; set; }
        
        public Boolean IsActive { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public User? User { get; set; }
        
        public WorkShift? WorkShift { get; set; }
    }
}