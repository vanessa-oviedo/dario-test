namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IBuyerZipCoverageReadRepository
    {
        Task<(int BuyerZipCoverageId, decimal DefaultQuoteAmount)?> GetCoverage(
            int buyerId, string zipCode, CancellationToken ct);
    }
}
