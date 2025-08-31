namespace Wheelzy.Application.Models
{
    public class CaseQuote
    {
        public long Id { get; set; }
        public int CaseId { get; set; }
        public int BuyerId { get; set; }
        public decimal Amount { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
