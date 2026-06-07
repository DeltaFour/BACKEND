using DeltaFour.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class PunchByEmailDto : PunchDto
    {
        public string? Justification { get; set; }

        public string? FileBase64 { get; set; }

        public string? Observation { get; set; }
    }
}
