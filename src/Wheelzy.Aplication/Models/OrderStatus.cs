namespace Wheelzy.Application.Models
{
    public class OrderStatus
    {
        public int StatusId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<OrderStatusHistory> Histories { get; set; } = new List<OrderStatusHistory>();
        public ICollection<Order> CurrentOrders { get; set; } = new List<Order>();
    }
}
