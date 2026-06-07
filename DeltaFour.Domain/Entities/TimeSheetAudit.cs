namespace DeltaFour.Domain.Entities;

public class TimeSheetAudit : BaseEntity
{
    public Guid TimeSheetId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Operation { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public TimeSheet? TimeSheet { get; set; }
}
