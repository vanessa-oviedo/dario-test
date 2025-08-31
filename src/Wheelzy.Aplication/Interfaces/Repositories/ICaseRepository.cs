using Wheelzy.Application.Interfaces.Queries;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    /// <summary>
    /// Acceso a casos de venta y sus colecciones (Quotes y StatusHistory).
    /// </summary>
    public interface ICaseRepository
    {
        Task<SellCase?> GetByIdAsync(int caseId, bool includeRelated = true, CancellationToken ct = default);

        Task AddAsync(SellCase entity, CancellationToken ct = default);
        Task UpdateAsync(SellCase entity, CancellationToken ct = default);

        // Lecturas específicas
        Task<CaseQuote?> GetCurrentQuoteAsync(int caseId, CancellationToken ct = default);
        Task<CaseStatusHistory?> GetCurrentStatusAsync(int caseId, CancellationToken ct = default);

        // Consulta de resúmenes con filtros y paginado
        Task<IReadOnlyList<CaseSummaryDto>> SearchSummariesAsync(
            CaseSearchFilter filter,
            CancellationToken ct = default);

        // Útil para saber si existe (y evitar traer todo el agregado)
        Task<bool> ExistsAsync(int caseId, CancellationToken ct = default);
    }
}
