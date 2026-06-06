using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class PunchByEmailDto : PunchDto
    {
        public String? Justification { get; set; }

        public String? FileBase64 { get; set; }

        public String? Observation { get; set; }
    }
}
