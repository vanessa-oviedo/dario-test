using Wheelzy.Application.DTOs.Common;

namespace Wheelzy.API.Requests
{
    public sealed class SearchOrdersRequest
    {
        public DateTime? CreatedFromUtc { get; init; }

        public DateTime? CreatedToUtc { get; init; }

        public List<int>? CustomerIds { get; init; }

        public List<int>? BuyerIds { get; init; }

        public List<int>? Statuses { get; init; }

        public string? ZipCode { get; init; }

        public PageRequest Page { get; init; } = new();
    }
}
