using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.Interfaces.Repositories;

namespace Wheelzy.Application.Interfaces.Service;

public interface IOrderService
{
    Task<int> Create(CreateSellCaseDto dto, CancellationToken ct = default);
    Task<PageResult<SellCaseSummaryDto>> SearchSummariesAsync(SearchCasesRequest req, CancellationToken ct = default);
}
