using Wheelzy.Application.Interfaces.Queries;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int caseId, bool includeRelated = true, CancellationToken ct = default);

        Task UpdateAsync(Order entity, CancellationToken ct = default);

        Task<CaseQuote?> GetCurrentQuoteAsync(int caseId, CancellationToken ct = default);
        Task<CaseStatusHistory?> GetCurrentStatusAsync(int caseId, CancellationToken ct = default);

        Task<List<OrderCurrentSummaryDto>> GetCurrentSummariesAsync(
            CancellationToken ct = default);

        Task<bool> ExistsAsync(int caseId, CancellationToken ct = default);

        Task<int> AddAsync(int customerId, int carId, string zipCode, DateTime now, CancellationToken token);
        Task SetCurrentStatusAsync(long orderId, int statusId, DateTime? statusDateUtc, string changedBy, CancellationToken ct);

        Task<string> GetOrderZipAsync(long orderId, CancellationToken ct);
        Task SetCurrentBuyerQuoteAsync(long orderId, long? orderBuyerQuoteId, CancellationToken o);
        Task<IEnumerable<SellCaseSummaryDto>> SearchSummariesAsync(OrderSearchFilter filter, CancellationToken ct);
    }

    public sealed record SellCaseSummaryDto(
        long OrderId,
        DateTime CreatedAtUtc,
        short CarYear,
        string Make,
        string Model,
        string SubModel,
        string? CurrentBuyerName,
        decimal? CurrentQuoteAmount,
        string? CurrentStatusName,
        DateTime? CurrentStatusDateUtc
    );
}
