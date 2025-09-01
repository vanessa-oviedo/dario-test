namespace Wheelzy.Application.Models
{
    public class CarMake
    {
        public int MakeId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<CarModel> Models { get; set; } = new List<CarModel>();
    }
}
