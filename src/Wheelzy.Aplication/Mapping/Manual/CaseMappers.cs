using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Mapping.Manual;

public static class CaseMappers
{
    /// <summary>
    /// Mapea un agregado SellCase a su detalle (sin buyer names).
    /// Reutiliza los datos de catálogo provistos por parámetros.
    /// </summary>
    public static SellCaseDetailDto ToDetailDto(
        SellCase agg,
        Car car,
        string? make,
        string? model,
        string? subModel)
    {
        var currentQuote = agg.Quotes.FirstOrDefault(q => q.IsCurrent);
        var currentStatus = agg.StatusHistory.FirstOrDefault(s => s.IsCurrent);

        return new SellCaseDetailDto
        {
            Id                    = agg.Id,
            CustomerId            = agg.CustomerId,
            CarId                 = agg.CarId,
            CarYear               = car.Year,
            Make                  = make  ?? string.Empty,
            Model                 = model ?? string.Empty,
            SubModel              = subModel ?? string.Empty,
            ZipCode               = agg.ZipCode,
            CreatedAtUtc          = agg.CreatedAtUtc,
            CurrentQuote          = currentQuote is null ? null : new CaseQuoteDto
            {
                Id            = currentQuote.Id,
                BuyerId       = currentQuote.BuyerId,
                BuyerName     = string.Empty,
                Amount        = currentQuote.Amount,
                IsCurrent     = true,
                CreatedAtUtc  = currentQuote.CreatedAtUtc
            },
            CurrentStatus         = currentStatus?.Status.ToString(),
            CurrentStatusDateUtc  = currentStatus?.StatusDateUtc,
            Quotes = agg.Quotes
                .OrderByDescending(q => q.IsCurrent)
                .ThenByDescending(q => q.CreatedAtUtc)
                .Select(q => new CaseQuoteDto
                {
                    Id            = q.Id,
                    BuyerId       = q.BuyerId,
                    BuyerName     = string.Empty,
                    Amount        = q.Amount,
                    IsCurrent     = q.IsCurrent,
                    CreatedAtUtc  = q.CreatedAtUtc
                })
                .ToList(),
            StatusHistory = agg.StatusHistory
                .OrderByDescending(s => s.IsCurrent)
                .ThenByDescending(s => s.CreatedAtUtc)
                .Select(s => new CaseStatusHistoryDto
                {
                    Id             = s.Id,
                    Status         = s.Status,
                    StatusDateUtc  = s.StatusDateUtc,
                    ChangedBy      = s.ChangedBy,
                    IsCurrent      = s.IsCurrent,
                    CreatedAtUtc   = s.CreatedAtUtc
                })
                .ToList()
        };
    }

    /// <summary>
    /// Simple helper para mapear un resumen a DTO de Application desde el DTO de Interfaces/Queries.
    /// </summary>
    public static CaseSummaryDto ToSummaryDto(Wheelzy.Application.Interfaces.Queries.CaseSummaryDto s) => new()
    {
        CaseId               = s.CaseId,
        Year                 = s.Year,
        Make                 = s.Make,
        Model                = s.Model,
        SubModel             = s.SubModel,
        ZipCode              = s.ZipCode,
        CurrentBuyer         = s.CurrentBuyer,
        CurrentQuote         = s.CurrentQuote,
        CurrentStatus        = s.CurrentStatus,
        CurrentStatusDateUtc = s.CurrentStatusDateUtc
    };
}
