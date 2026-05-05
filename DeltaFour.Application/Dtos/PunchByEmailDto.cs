using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class PunchByEmailDto
    {
        [Required]
        public String Email { get; set; }
        
        [Required]
        public String Password { get; set; }
        
        [Required]
        public ShiftType ShiftType { get; set; }
        
        [Required]
        public DateTime TimePunched { get; set; }
        
        [Required]
        public PunchType Type { get; set; }
    }
}
