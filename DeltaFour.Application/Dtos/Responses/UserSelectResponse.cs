namespace DeltaFour.Application.Dtos.Responses
{
    public class UserSelectResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? DepartmentName { get; internal set; }
    }
}
