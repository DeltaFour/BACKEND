namespace DeltaFour.Application.Dtos.TimeSheet;

public class TimeSheetListItemDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public bool SignedByEmployee { get; set; }
    public bool SignedByHR { get; set; }
    public DateTime CreatedAt { get; set; }
}
