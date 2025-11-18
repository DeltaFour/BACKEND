using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class PunchDto
    {
        [Required]
        public PunchType Type { get; set; }
        
        [Required]
        public DateTime TimePunched { get; set; }
        
        [Required]
        public String ImageBase64 { get; set; }
        
        [Required]
        public ShiftType ShiftType { get; set; }
        
        public Double latitude { get; set; }
        
        public Double longitude { get; set; }
        
        
    }
}
