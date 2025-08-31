using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.DTOs.Common;

namespace Wheelzy.Application.Interfaces.Service;

public interface ICaseService
{
    Task<int> CreateSellCaseAsync(CreateSellCaseDto dto, CancellationToken ct = default);
    Task<PageResult<CaseSummaryDto>> SearchSummariesAsync(SearchCasesRequest req, CancellationToken ct = default);
    Task<SellCaseDetailDto?> GetDetailAsync(int caseId, CancellationToken ct = default);
}
