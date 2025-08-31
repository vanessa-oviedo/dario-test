using Wheelzy.Application.Enums;

namespace Wheelzy.Application.DTOs.Case;

public sealed class ChangeCaseStatusDto
{
    public int CaseId { get; init; }
    public CaseStatus NewStatus { get; init; }
    public DateTime? StatusDateUtc { get; init; }
    public string ChangedBy { get; init; } = "system";
}
