using System.ComponentModel.DataAnnotations;

namespace DeltaFour.Application.Dtos
{
    public class WorkShiftUpdateDto : WorkShiftDto
    {
        [Required]
        public Guid Id { get; set; }
    }
}
