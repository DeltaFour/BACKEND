namespace DeltaFour.Domain.Entities;

public class Employee : BaseEntity
{
    public string? Name { get; set; }
    
    public string? Email { get; set; }
    
    public Guid? RoleId { get; set; }
    
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
    
    public Role? Role { get; set; }
    
    public EmployeeAuth? EmployeeAuth { get; set; }
    
    public Company? Company { get; set; }
    
    public List<EmployeeShift>? EmployeeShifts { get; set; }
    
    public List<EmployeeAttendance>? EmployeeAttendances { get; set; }
    
    public List<EmployeeFace>? EmployeeFaces { get; set; }
}