namespace Wheelzy.Application.Interfaces.Queries
{
    public sealed class CaseSearchFilter
    {
        public DateTime? CreatedFromUtc { get; init; }
        public DateTime? CreatedToUtc { get; init; }
        public IEnumerable<int>? CustomerIds { get; init; }
        public IEnumerable<int>? BuyerIds { get; init; }
        public IEnumerable<int>? Statuses { get; init; } // usar enum int (CaseStatus) para no acoplar aquí
        public string? ZipCode { get; init; }
        public int? Skip { get; init; }
        public int? Take { get; init; }
    }
}
