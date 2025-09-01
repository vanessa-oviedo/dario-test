namespace Wheelzy.Application.DTOs.Order;

public sealed class SetCurrentQuoteDto
{
    public int CaseId { get; init; }
    public int BuyerId { get; init; }
    public DateTime? WhenUtc { get; init; }
}
