namespace Wheelzy.Application.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>Persiste todos los cambios pendientes de la unidad de trabajo actual.</summary>
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
