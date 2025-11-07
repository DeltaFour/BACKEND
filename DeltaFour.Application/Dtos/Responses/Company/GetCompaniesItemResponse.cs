namespace DeltaFour.Application.Dtos.Responses.Company;

public class GetCompaniesItemResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Cnpj { get; set; }
    public bool IsActive { get; set; }
}
