namespace DeltaFour.Application.Dtos.Requests;

public class RegisterCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public UserRequest User { get; set; }
}
