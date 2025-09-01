using Wheelzy.Application.Interfaces.Queries;

namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<int> Add(int customerId, int carId, string zipCode, DateTime now, CancellationToken token);
        Task SetCurrentStatus(int orderId, int statusId, DateTime? statusDateUtc, CancellationToken ct);

        Task<string> GetOrderZip(int orderId, CancellationToken ct);
        Task SetCurrentBuyerQuote(int orderId, int? orderBuyerQuoteId, CancellationToken o);
        Task<IEnumerable<OrderSummaryDto>> SearchSummaries(OrderSearchFilter filter, CancellationToken ct);
        Task<bool> Exists(int orderId);
    }

    public sealed record OrderSummaryDto(
        int OrderId,
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
