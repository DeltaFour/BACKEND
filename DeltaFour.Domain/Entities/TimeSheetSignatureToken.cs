using DeltaFour.Domain.Enum;

namespace DeltaFour.Domain.Entities;

public class TimeSheetSignatureToken : BaseEntity
{
    public Guid TimeSheetId { get; set; }
    public SignerType SignerType { get; set; }
    public Guid SignerUserId { get; set; }
    public string Token { get; set; }
    public string Email { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    public TimeSheet? TimeSheet { get; set; }
}
