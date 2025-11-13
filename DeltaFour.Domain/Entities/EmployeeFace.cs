namespace DeltaFour.Domain.Entities
{
    public class EmployeeFace(Guid employeeId, string faceTemplate, Guid createdBy) : BaseEntity
    {
        public Guid EmployeeId { get; set; } = employeeId;

        public string FaceTemplate { get; set; } = faceTemplate;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid? UpdatedBy { get; set; }
        
        public Guid CreatedBy { get; set; } = createdBy;

        public Employee? Employee { get; set; }

    }
}