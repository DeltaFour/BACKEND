namespace DeltaFour.Domain.Entities
{
    public class EmployeeFace : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        
        public string? FaceTemplate { get; set; }
        
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid? UpdatedBy { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public Employee? Employee { get; set; }
    }
}