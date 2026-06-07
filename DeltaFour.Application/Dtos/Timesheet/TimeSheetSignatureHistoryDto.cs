namespace DeltaFour.Application.Dtos.TimeSheet;

public class TimeSheetSignatureHistoryDto
{
    public List<TimeSheetSignatureItemDto> Signatures { get; set; } = new();
    public List<TimeSheetSignatureRequestItemDto> Requests { get; set; } = new();
}

public class TimeSheetSignatureItemDto
{
    public string SignerType { get; set; } = string.Empty;
    public string SignerName { get; set; } = string.Empty;
    public string SignerCpf { get; set; } = string.Empty;
    public string SignerEmail { get; set; } = string.Empty;
    public DateTime SignedAtUtc { get; set; }
    public string SignerIp { get; set; } = string.Empty;
    public string TimeSheetHash { get; set; } = string.Empty;
}

public class TimeSheetSignatureRequestItemDto
{
    public string SignerType { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
}
