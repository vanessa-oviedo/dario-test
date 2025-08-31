namespace Wheelzy.Application.DTOs.Case;

public sealed class CaseQuoteDto
{
    public long Id { get; init; }
    public int BuyerId { get; init; }
    public string BuyerName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public bool IsCurrent { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
