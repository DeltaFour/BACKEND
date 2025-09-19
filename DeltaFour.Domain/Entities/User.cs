namespace DeltaFour.Domain.Entities;

public class User : BaseEntity
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Cellphone { get; set; }
    public Guid CompanyId { get; set; }
    public bool IsActive { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsAllowedBypassCoord {  get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Guid CreatedBy { get; set; }
    public UserAuth? UserAuth { get; set; }
    public Company? Company { get; set; }
}