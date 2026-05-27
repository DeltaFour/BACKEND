using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class PunchByEmailDto : PunchDto
    {
        [Required]
        public String Email { get; set; }

        [Required]
        public String Password { get; set; }

        public String? Justification { get; set; }

        public String? FileBase64 { get; set; }

        public String? Observation { get; set; }
    }
}
