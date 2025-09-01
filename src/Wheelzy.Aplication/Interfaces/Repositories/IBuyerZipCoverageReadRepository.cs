namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IBuyerZipCoverageReadRepository
    {
        Task<IReadOnlyList<CoverageProjection>> GetByZipAsync(string zipCode, CancellationToken ct = default);

        Task<(int BuyerZipCoverageId, decimal DefaultQuoteAmount)?> GetCoverage(
            int buyerId, string zipCode, CancellationToken ct);
    }
}
