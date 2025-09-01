namespace Wheelzy.Application.Interfaces.Queries
{
    public sealed class OrderSearchFilter
    {
        public DateTime? CreatedFromUtc { get; init; }
        public DateTime? CreatedToUtc { get; init; }
        public IEnumerable<int>? CustomerIds { get; init; }
        public IEnumerable<int>? BuyerIds { get; init; }
        public IEnumerable<int>? Statuses { get; init; }
        public string? ZipCode { get; init; }
        public int? Skip { get; init; }
        public int? Take { get; init; }
        public bool? IsActive { get; set; }
    }
}
