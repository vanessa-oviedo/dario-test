using Wheelzy.Application.DTOs.Common;

namespace Wheelzy.Application.Requests;

public sealed record SearchOrdersRequest(DateTime? CreatedFromUtc, DateTime? CreatedToUtc, IEnumerable<int>? CustomerIds, IEnumerable<int>? BuyerIds, IEnumerable<int>? Statuses, string? ZipCode)
{
    public PageRequest Page { get; init; } = new();
}

public sealed record CreateQuoteRequest(
    int OrderId,
    int BuyerId,
    string ZipCode,
    decimal? AmountOverride = null,
    string? CreatedBy = null,
    DateTime? NowUtc = null
);