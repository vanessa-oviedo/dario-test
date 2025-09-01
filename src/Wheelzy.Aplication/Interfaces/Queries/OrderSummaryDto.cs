namespace Wheelzy.Application.Interfaces.Queries
{
    public sealed record OrderCurrentSummaryDto(
        long OrderId,
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
