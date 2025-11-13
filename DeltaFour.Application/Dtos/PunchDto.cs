using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class PunchDto
    {
        public Double latitude { get; set; }
        
        public Double longitude { get; set; }
        
        [Required]
        public PunchType Type { get; set; }
        
        [Required]
        public DateTime TimePunched { get; set; }
        
        [Required]
        public ShiftType ShiftType { get; set; }
        
        [Required]
        public String ImageBase64 { get; set; }
        
    }
}
