namespace Wheelzy.Application.Models
{
    public class Car
    {
        public int Id { get; set; }
        public short Year { get; set; }   // Validaremos rango en Services o Configurations
        public int SubModelId { get; set; }
    }
}
