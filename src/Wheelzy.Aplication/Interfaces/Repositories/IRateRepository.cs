namespace Wheelzy.Application.Interfaces.Repositories
{
    /// <summary>
    /// Tarifas base por ZIP y Buyer para generar cotizaciones iniciales.
    /// No definimos un modelo explícito: devolvemos tuplas ligeras.
    /// </summary>
    public interface IRateRepository
    {
        /// <returns>Lista (BuyerId, Amount) para un ZIP dado.</returns>
        Task<IReadOnlyList<(int BuyerId, decimal Amount)>> GetBaseRatesByZipAsync(
            string zipCode,
            CancellationToken ct = default);
    }
}
