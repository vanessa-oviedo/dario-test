namespace Wheelzy.Application.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Balance { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
