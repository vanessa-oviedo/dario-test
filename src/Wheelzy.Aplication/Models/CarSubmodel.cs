namespace Wheelzy.Application.Models
{
    public class CarSubmodel
    {
        public int SubmodelId { get; set; }
        public int ModelId { get; set; }
        public string Name { get; set; } = null!;
        public CarModel Model { get; set; } = null!;
        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
