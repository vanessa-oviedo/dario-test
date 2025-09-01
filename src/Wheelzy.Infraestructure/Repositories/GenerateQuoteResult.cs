namespace Wheelzy.Infrastructure.Repositories
{
    public sealed record GenerateQuoteResult(
        long OrderBuyerQuoteId,
        decimal AmountUsed
    );
}