using Wheelzy.Application.DTOs.Order;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Queries;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Requests;

namespace Wheelzy.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IOrderStatusRepository _statusReaderRepository;
    private readonly IOrderStatusHistoryRepository _historyWriterRepository;

    public OrderService(IOrderRepository orderRepository, IUnitOfWork uow, IClock clock, IOrderStatusRepository statusReaderRepository, IOrderStatusHistoryRepository historyWriterRepository)
    {
        _orderRepository   = orderRepository;
        _uow     = uow;
        _clock   = clock;
        _statusReaderRepository = statusReaderRepository;
        _historyWriterRepository = historyWriterRepository;
    }

    public async Task<int> Create(CreateOrderDto dto, CancellationToken token = default)
    {
        var now = dto.CreatedAtUtc ?? DateTime.UtcNow;
        var orderId = 0;
        int? initialStatusId = null;

        await _uow.ExecuteInTransactionAsync(async (_) =>
        {
            orderId = await _orderRepository.Add(dto.CustomerId, dto.CarId, dto.ZipCode, now, token);
            await _uow.SaveChanges(token);
            
            if (dto.SetInitialStatus)
            {
                initialStatusId = await _statusReaderRepository.GetStatusIdByName("Pending Acceptance", token);
                if (initialStatusId.HasValue)
                {
                    await _orderRepository.SetCurrentStatus(orderId, initialStatusId.Value, now, token);
                    await _historyWriterRepository.Add(orderId, initialStatusId.Value, now, token);
                }
            }

            await _uow.SaveChanges(token);
        }, token);

        return orderId;
    }


    public async Task<PageResult<OrderSummaryDto>> SearchSummaries(SearchOrdersRequest req, CancellationToken token = default)
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

        var rows = await _orderRepository.GetOrders(filter, token);
        
        return new PageResult<OrderSummaryDto>
        {
            Items = rows.ToList(),
            Total = rows.Count()
        };
    }
}
