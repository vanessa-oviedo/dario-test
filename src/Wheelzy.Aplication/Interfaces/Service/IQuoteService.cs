using Wheelzy.Application.DTOs.Case;

namespace Wheelzy.Application.Interfaces.Service;

public interface IQuoteService
{
    Task<int> GenerateBaseQuotesAsync(CreateQuoteCommand dto, CancellationToken ct = default);

    Task SetCurrentQuoteAsync(long orderId, long? orderBuyerQuoteId, CancellationToken t = default);
}
