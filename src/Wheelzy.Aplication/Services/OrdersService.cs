using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Queries;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Mapping.Manual;
using OrderSummaryDto = Wheelzy.Application.DTOs.Case.CaseSummaryDto;

namespace Wheelzy.Application.Services;

public sealed class OrdersService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IOrderStatusReadRepository _statusReader;
    private readonly IOrderStatusHistoryRepository _historyWriter;

    public OrdersService(IOrderRepository _orders, IUnitOfWork uow, IClock clock)
    {
        this._orders   = _orders;
        _uow     = uow;
        _clock   = clock;
    }

    public async Task<int> Create(CreateSellCaseDto dto, CancellationToken ct = default)
    {
        var now = dto.CreatedAtUtc ?? DateTime.UtcNow;
        var orderId = 0;
        int? initialStatusId = null;

        await _uow.ExecuteInTransactionAsync(async (_) =>
        {
            orderId = await _orders.AddAsync(dto.CustomerId, dto.CarId, dto.ZipCode, now, ct);
            await _uow.SaveChangesAsync(ct);
            
            if (dto.SetInitialStatus)
            {
                initialStatusId = await _statusReader.GetStatusIdByNameAsync("Pending Acceptance", ct);
                if (initialStatusId.HasValue)
                {
                    await _orders.SetCurrentStatusAsync(orderId, initialStatusId.Value, now, dto.CreatedBy ?? "system", ct);
                    await _historyWriter.AddAsync(orderId, initialStatusId.Value, now, dto.CreatedBy ?? "system", ct);
                }
            }

            await _uow.SaveChangesAsync(ct);
        }, ct);

        return orderId;
    }


    public async Task<PageResult<SellCaseSummaryDto>> SearchSummariesAsync(SearchCasesRequest req, CancellationToken ct = default)
    {
        var filter = new OrderSearchFilter
        {
            CreatedFromUtc = req.CreatedFromUtc,
            CreatedToUtc   = req.CreatedToUtc,
            CustomerIds    = req.CustomerIds,
            BuyerIds       = req.BuyerIds,
            Statuses       = req.Statuses,
            ZipCode        = string.IsNullOrWhiteSpace(req.ZipCode) ? null : req.ZipCode.Trim(),
            Skip           = req.Page.Skip,
            Take           = req.Page.Take
        };

        var rows = await _orders.SearchSummariesAsync(filter, ct);
        
        return new PageResult<SellCaseSummaryDto>
        {
            Items = rows.ToList(),
            Total = rows.Count()
        };
    }
}
