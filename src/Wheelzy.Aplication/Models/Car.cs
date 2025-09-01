namespace Wheelzy.Application.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public short Year { get; set; }

        public int SubmodelId { get; set; }
        public CarSubmodel Submodel { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CustomerCar> CustomerCars { get; set; } = new List<CustomerCar>();
    }
}
