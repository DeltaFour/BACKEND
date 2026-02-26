using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class CanPunchDto
    {
        [Required]
        public TimeOnly TimePunched { get; set; }
        
        [Required]
        public PunchType PunchType { get; set; }
    }
}
