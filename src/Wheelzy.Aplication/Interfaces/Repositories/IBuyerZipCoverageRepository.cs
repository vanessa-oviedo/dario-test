namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IBuyerZipCoverageRepository
    {
        Task<(int BuyerZipCoverageId, decimal DefaultQuoteAmount)?> GetCoverage(
            int buyerId, string zipCode, CancellationToken ct);
    }
}
