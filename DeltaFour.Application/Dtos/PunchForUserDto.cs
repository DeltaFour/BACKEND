using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class PunchForUserDto
    {
        [Required]
        public PunchType Type { get; set; }
        
        [Required]
        public DateTime TimePunched { get; set; }
        
        [Required]
        public ShiftType ShiftType { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
    }
}
