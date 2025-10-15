namespace DeltaFour.Application.Dtos
{
    public class EmployeeUpdateDto
    {
        public Guid Id { get; set; }
        
        public String? Name { get; set; }
        
        public String? RoleName { get; set; }
        
        public String? CellPhone { get; set; }
    }
}
