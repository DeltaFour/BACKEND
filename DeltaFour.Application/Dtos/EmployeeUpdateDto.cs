namespace DeltaFour.Application.Dtos
{
    public class EmployeeUpdateDto
    {
        public Guid Id { get; set; }
        
        public String? Name { get; set; }
        
        public String? CellPhone { get; set; }
        
        public Boolean IsAllowedBypassCoord { get; set; }
        
        public List<EmployeeShiftDto> EmployeeShift { get; set; }
    }
}
