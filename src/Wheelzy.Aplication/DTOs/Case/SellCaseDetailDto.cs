namespace Wheelzy.Application.DTOs.Case;

public sealed class SellCaseDetailDto
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public int CarId { get; init; }
    public short CarYear { get; init; }
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string SubModel { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public CaseQuoteDto? CurrentQuote { get; init; }
    public string? CurrentStatus { get; init; }
    public DateTime? CurrentStatusDateUtc { get; init; }
    public IReadOnlyList<CaseQuoteDto> Quotes { get; init; } = Array.Empty<CaseQuoteDto>();
    public IReadOnlyList<CaseStatusHistoryDto> StatusHistory { get; init; } = Array.Empty<CaseStatusHistoryDto>();
}
