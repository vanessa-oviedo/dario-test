namespace Wheelzy.Application.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public long CarId { get; set; }
        public string ZipCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public int? CurrentStatusId { get; set; }
        public DateTime? CurrentStatusDate { get; set; }
        public string? CurrentStatusChangedBy { get; set; }

        public long? CurrentOrderBuyerQuoteId { get; set; }

        public Customer Customer { get; set; } = null!;
        public Car Car { get; set; } = null!;
        public ZipCode Zip { get; set; } = null!;
        public OrderStatus? CurrentStatus { get; set; }
        public OrderBuyerQuote? CurrentOrderBuyerQuote { get; set; }

        public long? CurrentOrderBuyerQuoteOrderId { get; set; }

        public ICollection<OrderBuyerQuote> OrderBuyerQuotes { get; set; } = new List<OrderBuyerQuote>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
