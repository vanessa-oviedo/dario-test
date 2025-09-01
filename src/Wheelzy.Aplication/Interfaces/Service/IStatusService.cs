namespace Wheelzy.Application.Interfaces.Service;

public interface IStatusService
{
    Task UpdateStatusAsync(
        long orderId, int newStatusId, DateTime? statusDate, string changedBy,
        CancellationToken t = default);
}
