using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos.Requests;

public class CreateCompanyRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Cnpj { get; set; } = string.Empty;

    [Required]
    public UserRequest? User { get; set; }
}