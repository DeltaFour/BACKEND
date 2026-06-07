namespace DeltaFour.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }

    public string Email { get; set; }

    public Guid? RoleId { get; set; }

    public string Password { get; set; }

    public string? Cellphone { get; set; }

    public string? Cpf { get; set; }

    public Guid CompanyId { get; set; }

    public Guid? DepartmentId { get; set; }

    public bool IsActive { get; set; }

    public bool IsConfirmed { get; set; }

    public bool IsAllowedBypassCoord { get; set; }

    public bool IsAllowedBypassFacial { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public Guid CreatedBy { get; set; }

    public Role? Role { get; set; }

    public UserAuth? UserAuth { get; set; }

    public Company Company { get; set; }

    public Department? Department { get; set; }

    public List<UserShift>? UserShifts { get; set; }

    public List<UserAttendance>? UserAttendances { get; set; }

    public List<UserFace>? UserFaces { get; set; }
}
