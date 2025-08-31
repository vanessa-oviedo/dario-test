namespace Wheelzy.Application.DTOs.Case;

public sealed class SetCurrentQuoteDto
{
    public int CaseId { get; init; }
    public int BuyerId { get; init; }
    public DateTime? WhenUtc { get; init; }
}
