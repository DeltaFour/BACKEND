using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class CanPunchDto
    {
        [Required]
        public DateTime TimePunched { get; set; }
    }
}
