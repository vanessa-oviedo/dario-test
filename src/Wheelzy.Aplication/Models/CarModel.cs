namespace Wheelzy.Application.Models
{
    public class CarModel
    {
        public int ModelId { get; set; }
        public int MakeId { get; set; }
        public string Name { get; set; } = null!;
        public CarMake Make { get; set; } = null!;
        public ICollection<CarSubmodel> Submodels { get; set; } = new List<CarSubmodel>();
    }
}
