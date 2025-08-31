using Wheelzy.Application.DTOs.Common;

namespace Wheelzy.Application.DTOs.Case;

public sealed class SearchCasesRequest
{
    public DateTime? CreatedFromUtc { get; init; }
    public DateTime? CreatedToUtc { get; init; }
    public IEnumerable<int>? CustomerIds { get; init; }
    public IEnumerable<int>? BuyerIds { get; init; }
    public IEnumerable<int>? Statuses { get; init; }
    public string? ZipCode { get; init; }
    public PageRequest Page { get; init; } = new();
}
