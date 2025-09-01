using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.Application.Services;

public sealed class StatusService : IStatusService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
    private readonly IOrderStatusReadRepository _orderStatusReadRepository;

    public StatusService(IOrderRepository orderRepository, IOrderStatusHistoryRepository orderStatusHistoryRepository, 
        IUnitOfWork uow, IClock clock, 
        IOrderStatusReadRepository orderStatusReadRepository)
    {
        _orderRepository = orderRepository;
        _uow   = uow;
        _clock = clock;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _orderStatusReadRepository = orderStatusReadRepository;
    }

    public async Task UpdateStatus(
        int orderId,
        int newStatusId,
        DateTime? statusDate,
        CancellationToken t = default)
    {
        var orderExists = await _orderRepository.Exists(orderId);
        if (orderExists == false)
        {
            throw new DirectoryNotFoundException("Invalid Order");
        }

        var pickedUpId = await _orderStatusReadRepository.GetStatusIdByName("Picked Up", t);

        if (pickedUpId.HasValue && newStatusId == pickedUpId.Value && statusDate is null)
            throw new InvalidOperationException("Picked Up requires a status date.");

        await _uow.ExecuteInTransactionAsync(async (_) =>
        {
            await _orderRepository.SetCurrentStatus(orderId, newStatusId, statusDate, t);
            await _orderStatusHistoryRepository.Add(orderId, newStatusId, DateTime.UtcNow, t);
            await _uow.SaveChanges(t);
        }, t);
    }
}
