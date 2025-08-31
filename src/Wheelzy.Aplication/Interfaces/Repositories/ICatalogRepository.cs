using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    /// <summary>
    /// Catálogo de Make/Model/SubModel y autos.
    /// </summary>
    public interface ICatalogRepository
    {
        Task<Make?> GetMakeAsync(int id, CancellationToken ct = default);
        Task<Model?> GetModelAsync(int id, CancellationToken ct = default);
        Task<SubModel?> GetSubModelAsync(int id, CancellationToken ct = default);
        Task<Car?> GetCarAsync(int id, CancellationToken ct = default);

        // Búsquedas ligeras para combos/autocomplete
        Task<IReadOnlyList<Make>> ListMakesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Model>> ListModelsByMakeAsync(int makeId, CancellationToken ct = default);
        Task<IReadOnlyList<SubModel>> ListSubModelsByModelAsync(int modelId, CancellationToken ct = default);
    }
}
