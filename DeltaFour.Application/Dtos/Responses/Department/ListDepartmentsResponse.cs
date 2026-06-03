namespace DeltaFour.Application.Dtos.Responses.Department;

public class ListDepartmentsResponse
{
    public List<GetDepartmentItemResponse> Departments { get; set; } = new();
}
