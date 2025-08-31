using Wheelzy.Application.DTOs.Case;

namespace Wheelzy.Application.Interfaces.Service;

public interface IQuoteService
{
    Task<int> GenerateBaseQuotesAsync(GenerateBaseQuotesDto dto, CancellationToken ct = default);
    Task SetCurrentQuoteAsync(SetCurrentQuoteDto dto, CancellationToken ct = default);
}
