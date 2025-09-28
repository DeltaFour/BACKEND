namespace DeltaFour.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Company? Company { get; set; }
        public User? User { get; set; }
    }
}
