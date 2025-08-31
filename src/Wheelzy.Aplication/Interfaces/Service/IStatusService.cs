using Wheelzy.Application.DTOs.Case;

namespace Wheelzy.Application.Interfaces.Service;

public interface IStatusService
{
    Task ChangeStatusAsync(ChangeCaseStatusDto dto, CancellationToken ct = default);
}
