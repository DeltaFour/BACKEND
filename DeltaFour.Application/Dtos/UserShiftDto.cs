namespace DeltaFour.Application.Dtos
{
    public class UserShiftDto
    {
        public Guid? Id { get; set; }

        public Guid ShiftId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Boolean IsActive { get; set; }
    }
}
