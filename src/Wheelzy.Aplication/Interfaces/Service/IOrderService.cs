using Wheelzy.Application.DTOs.Order;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.Requests;

namespace Wheelzy.Application.Interfaces.Service;

public interface IOrderService
{
    Task<int> Create(CreateSellCaseDto dto, CancellationToken ct = default);
    Task<PageResult<OrderSummaryDto>> SearchSummaries(SearchOrdersRequest req, CancellationToken ct = default);
}
