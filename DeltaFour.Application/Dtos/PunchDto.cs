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
    }
}
