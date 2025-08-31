using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.Enums;
using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Queries;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Mapping.Manual;
using Wheelzy.Application.Models;
using CaseSummaryDto = Wheelzy.Application.DTOs.Case.CaseSummaryDto;

namespace Wheelzy.Application.Services;

public sealed class CaseService : ICaseService
{
    private readonly ICaseRepository _cases;
    private readonly ICatalogRepository _catalog;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public CaseService(ICaseRepository cases, ICatalogRepository catalog, IUnitOfWork uow, IClock clock)
    {
        _cases   = cases;
        _catalog = catalog;
        _uow     = uow;
        _clock   = clock;
    }

    public async Task<int> CreateSellCaseAsync(CreateSellCaseDto dto, CancellationToken ct = default)
    {
        if (dto.CustomerId <= 0) throw new ArgumentException("CustomerId inválido.");
        if (dto.CarId <= 0) throw new ArgumentException("CarId inválido.");
        if (string.IsNullOrWhiteSpace(dto.ZipCode)) throw new ArgumentException("ZipCode requerido.");

        var car = await _catalog.GetCarAsync(dto.CarId, ct) 
                  ?? throw new InvalidOperationException("Car no encontrado.");

        var createdAt = dto.CreatedAtUtc ?? _clock.UtcNow;

        var sc = new SellCase
        {
            CustomerId   = dto.CustomerId,
            CarId        = car.Id,
            ZipCode      = dto.ZipCode.Trim(),
            CreatedAtUtc = createdAt,
            StatusHistory = new()
            {
                new CaseStatusHistory
                {
                    Status        = CaseStatus.PendingAcceptance,
                    ChangedBy     = "system",
                    IsCurrent     = true,
                    CreatedAtUtc  = createdAt
                }
            }
        };

        await _cases.AddAsync(sc, ct);
        await _uow.SaveChangesAsync(ct);
        return sc.Id;
    }

    public async Task<PageResult<CaseSummaryDto>> SearchSummariesAsync(SearchCasesRequest req, CancellationToken ct = default)
    {
        var filter = new CaseSearchFilter
        {
            CreatedFromUtc = req.CreatedFromUtc,
            CreatedToUtc   = req.CreatedToUtc,
            CustomerIds    = req.CustomerIds,
            BuyerIds       = req.BuyerIds,
            Statuses       = req.Statuses,
            ZipCode        = string.IsNullOrWhiteSpace(req.ZipCode) ? null : req.ZipCode.Trim(),
            Skip           = req.Page.Skip,
            Take           = req.Page.Take
        };

        var rows = await _cases.SearchSummariesAsync(filter, ct);
        var items = rows.Select(CaseMappers.ToSummaryDto).ToList();

        return new PageResult<CaseSummaryDto>
        {
            Items = items,
            Total = items.Count
        };
    }

    public async Task<SellCaseDetailDto?> GetDetailAsync(int caseId, CancellationToken ct = default)
    {
        var agg = await _cases.GetByIdAsync(caseId, includeRelated: true, ct);
        if (agg is null) return null;

        var car = await _catalog.GetCarAsync(agg.CarId, ct) ?? new Car { Id = agg.CarId };
        var sub = await _catalog.GetSubModelAsync(car.SubModelId, ct);
        var mdl = sub is not null ? await _catalog.GetModelAsync(sub.ModelId, ct) : null;
        var mk  = mdl is not null ? await _catalog.GetMakeAsync(mdl.MakeId, ct) : null;

        return CaseMappers.ToDetailDto(agg, car, mk?.Name, mdl?.Name, sub?.Name);
    }
}
