using Wheelzy.Application.Enums;

namespace Wheelzy.Application.DTOs.Case;

public sealed class CaseStatusHistoryDto
{
    public long Id { get; init; }
    public CaseStatus Status { get; init; }
    public string StatusName => Status.ToString();
    public DateTime? StatusDateUtc { get; init; }
    public string ChangedBy { get; init; } = string.Empty;
    public bool IsCurrent { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
