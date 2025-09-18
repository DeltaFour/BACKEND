using DeltaFour.Domain.Enums.User;

namespace DeltaFour.Domain.Entities;

public class User : BaseEntity
{
    public string? Name { get; set; }
    public string? Email { get; set; } 
    public string? Password { get; set; }
    public Roles?  Role {get; set;}
}