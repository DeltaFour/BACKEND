namespace DeltaFour.Application.Dtos.TimeSheet;

public class TimeSheetAuditDto
{
    public string Operation { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string OldValues { get; set; } = string.Empty;
    public string NewValues { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
