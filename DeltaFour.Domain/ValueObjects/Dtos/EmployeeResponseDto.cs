using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.ValueObjects.Dtos
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }

        public String Name {get; set;} 

        public String? Cellphone { get; set; }
        
        public String Email { get; set; }

        public String? RoleName { get; set; }

        public Boolean IsActive { get; set; }

        public Boolean IsAllowedBypassCoord { get; set; }

        public DateTime? LastLogin { get; set; }
        
        public List<EmployeeResponseShiftsDto>? ShiftDto { get; set; }
    }
}
