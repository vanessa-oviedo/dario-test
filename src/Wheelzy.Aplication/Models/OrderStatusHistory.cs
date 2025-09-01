namespace Wheelzy.Application.Models
{
    public class OrderStatusHistory
    {
        public long OrderStatusHistoryId { get; set; }
        public long OrderId { get; set; }
        public int StatusId { get; set; }
        public DateTime? StatusDate { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Order Order { get; set; } = null!;
        public OrderStatus Status { get; set; } = null!;
    }
}
