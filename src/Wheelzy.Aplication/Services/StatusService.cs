using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.Enums;
using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Services;

public sealed class StatusService : IStatusService
{
    private readonly ICaseRepository _cases;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public StatusService(ICaseRepository cases, IUnitOfWork uow, IClock clock)
    {
        _cases = cases;
        _uow   = uow;
        _clock = clock;
    }

    public async Task ChangeStatusAsync(ChangeCaseStatusDto dto, CancellationToken ct = default)
    {
        var agg = await _cases.GetByIdAsync(dto.CaseId, includeRelated: true, ct)
                  ?? throw new InvalidOperationException("Case no encontrado.");

        if (dto.NewStatus == CaseStatus.PickedUp && dto.StatusDateUtc is null)
            throw new InvalidOperationException("PickedUp requiere StatusDateUtc.");

        foreach (var s in agg.StatusHistory) s.IsCurrent = false;

        agg.StatusHistory.Add(new CaseStatusHistory
        {
            Status        = dto.NewStatus,
            StatusDateUtc = dto.StatusDateUtc,
            ChangedBy     = string.IsNullOrWhiteSpace(dto.ChangedBy) ? "system" : dto.ChangedBy,
            IsCurrent     = true,
            CreatedAtUtc  = _clock.UtcNow
        });

        await _cases.UpdateAsync(agg, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
