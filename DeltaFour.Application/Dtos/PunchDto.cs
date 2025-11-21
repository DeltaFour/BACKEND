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
        
        public Double Latitude { get; set; }
        
        public Double Longitude { get; set; }
        
        
    }
}
