using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities;

public class TimeSheetSignature : BaseEntity
{
    public Guid TimeSheetId { get; set; }
    public SignerType SignerType { get; set; }
    public Guid SignerUserId { get; set; }
    public string SignerName { get; set; }
    public string SignerCpf { get; set; }
    public string SignerEmail { get; set; }
    public DateTime SignedAtUtc { get; set; }
    public string SignerIp { get; set; }
    public string TimeSheetHash { get; set; }
    public TimeSheet? TimeSheet { get; set; }
}
