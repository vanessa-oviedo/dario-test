namespace Wheelzy.Application.Models
{
    public class SellCase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }   // En este scope es un entero simple
        public int CarId { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }

        // Colecciones navegables (sin lógica de negocio, se maneja en Services)
        public List<CaseQuote> Quotes { get; set; } = new();
        public List<CaseStatusHistory> StatusHistory { get; set; } = new();

        // Helpers de lectura (opcionales)
        public CaseQuote? CurrentQuote => Quotes.FirstOrDefault(q => q.IsCurrent);
        public CaseStatusHistory? CurrentStatus => StatusHistory.FirstOrDefault(s => s.IsCurrent);
    }
}
