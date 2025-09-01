using Wheelzy.Application.Requests;

namespace Wheelzy.Application.Interfaces.Service;

public interface IQuoteService
{
    Task<int> GenerateBaseQuotes(CreateQuoteRequest dto, CancellationToken token = default);

    Task SetCurrentQuote(int orderId, string zipCode, CancellationToken token = default);
}
