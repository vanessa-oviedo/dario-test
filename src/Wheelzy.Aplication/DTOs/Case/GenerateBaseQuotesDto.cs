namespace Wheelzy.Application.DTOs.Case;

public sealed record CreateQuoteCommand(
    int OrderId,
    int BuyerId,
    decimal? AmountOverride = null,
    string? CreatedBy = null,
    DateTime? NowUtc = null
);

public sealed record CreateQuoteResult(
    int OrderBuyerQuoteId,
    decimal AmountUsed
);
