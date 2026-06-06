namespace DeltaFour.Domain.Entities;

public class Department : BaseEntity
{
    public string? Name { get; set; }

    public Guid CompanyId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public Company Company { get; set; }

    public List<User>? Users { get; set; }
}
