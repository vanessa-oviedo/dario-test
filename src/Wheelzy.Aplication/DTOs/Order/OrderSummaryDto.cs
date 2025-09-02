namespace Wheelzy.Application.DTOs.Order
{
    public sealed record OrderSummaryDto(
        int OrderId,
        DateTime CreatedAt,
        short CarYear,
        string Make,
        string Model,
        string SubModel,
        string? CurrentBuyerName,
        decimal? CurrentQuoteAmount,
        string? CurrentStatusName,
        DateTime? CurrentStatusDate
    );
}
