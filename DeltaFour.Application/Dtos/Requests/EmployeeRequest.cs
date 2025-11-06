using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos.Requests;

public class EmployeeRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MaxLength(128)]
    [MinLength(3)]
    public string? Name { get; set; }

    [Required]
    [MaxLength(32)]
    [MinLength(8)]
    public string? Password { get; set; }
}
