using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.Application.Services;

public sealed class StatusService : IStatusService
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IOrderStatusHistoryRepository _history;
    private readonly IOrderStatusReadRepository _statusReader;

    public StatusService(IOrderRepository orders, IOrderStatusHistoryRepository history, IUnitOfWork uow, IClock clock, IOrderStatusReadRepository statusReader)
    {
        _orders = orders;
        _uow   = uow;
        _clock = clock;
        _history = history;
        _statusReader = statusReader;
    }

    public async Task UpdateStatusAsync(
        long orderId,
        int newStatusId,
        DateTime? statusDate,
        string changedBy,
        CancellationToken t = default)
    {
        // Buscar el ID real de "Picked Up" en la BD (sin números mágicos)
        var pickedUpId = await _statusReader.GetStatusIdByNameAsync("Picked Up", t);

        // Si el nuevo estado es Picked Up y no viene fecha => error de negocio
        if (pickedUpId.HasValue && newStatusId == pickedUpId.Value && statusDate is null)
            throw new InvalidOperationException("Picked Up requires a status date.");

        await _uow.ExecuteInTransactionAsync(async (_) =>
        {
            await _orders.SetCurrentStatusAsync(orderId, newStatusId, statusDate, changedBy, t);
            await _history.AddAsync(orderId, newStatusId, statusDate.Value, changedBy, t); //watchdout here statusDate is nullale
            await _uow.SaveChangesAsync(t);
        }, t);
    }

}
