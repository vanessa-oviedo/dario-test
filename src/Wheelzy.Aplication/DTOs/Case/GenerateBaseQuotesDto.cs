namespace Wheelzy.Application.DTOs.Case;

public sealed class GenerateBaseQuotesDto
{
    public int CaseId { get; init; }
    public bool SetBestAsCurrent { get; init; } = false;
}
