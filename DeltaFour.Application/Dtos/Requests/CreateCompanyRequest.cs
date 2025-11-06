using System.ComponentModel.DataAnnotations;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Dtos.Requests;

public class CreateCompanyRequest
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Cnpj { get; set; }

    [Required]
    public EmployeeRequest? Employee { get; set; }
}