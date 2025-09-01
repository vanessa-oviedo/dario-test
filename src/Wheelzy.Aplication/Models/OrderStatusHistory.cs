namespace Wheelzy.Application.Models
{
    public class OrderStatusHistory
    {
        public int OrderStatusHistoryId { get; set; }
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public Order Order { get; set; } = null!;
        public OrderStatus Status { get; set; } = null!;
    }
}
