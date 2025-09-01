namespace Wheelzy.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<object, Task> action, CancellationToken ct);

        Task<int> SaveChanges(CancellationToken ct = default);
    }
}
