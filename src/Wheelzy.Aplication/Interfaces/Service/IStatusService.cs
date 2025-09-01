namespace Wheelzy.Application.Interfaces.Service;

public interface IStatusService
{
    Task UpdateStatus(
        int orderId, int newStatusId, DateTime? statusDate, CancellationToken t = default);
}
