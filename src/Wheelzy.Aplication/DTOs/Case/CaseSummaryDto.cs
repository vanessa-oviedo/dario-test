namespace Wheelzy.Application.DTOs.Case;

public sealed class CaseSummaryDto
{
    public int CaseId { get; init; }
    public short Year { get; init; }
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string SubModel { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string? CurrentBuyer { get; init; }
    public decimal? CurrentQuote { get; init; }
    public string? CurrentStatus { get; init; }
    public DateTime? CurrentStatusDateUtc { get; init; }
}
