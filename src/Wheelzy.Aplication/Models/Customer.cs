namespace Wheelzy.Application.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public decimal Balance { get; set; }

        public ZipCode ZipCodeRef { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<CustomerCar> CustomerCars { get; set; } = new List<CustomerCar>();
    }
}
