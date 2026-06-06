namespace DeltaFour.Domain.Entities;

public class TimeSheet : BaseEntity
{
    public Guid UserId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public bool SignedByEmployee { get; set; }
    public DateTime? EmployeeSignedAt { get; set; }
    public bool SignedByHR { get; set; }
    public DateTime? HRSignedAt { get; set; }
    public Guid? SignedByHRUserId { get; set; }
    public string? SignedByHRUserName { get; set; }
    public User? User { get; set; }
    public List<TimeSheetSignature>? Signatures { get; set; }
    public List<TimeSheetSignatureToken>? SignatureTokens { get; set; }
    public List<TimeSheetAudit>? Audits { get; set; }
}
