namespace DeltaFour.Application.Dtos.Responses.Department;

public class GetDepartmentItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
