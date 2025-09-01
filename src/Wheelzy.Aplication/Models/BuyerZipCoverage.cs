
namespace Wheelzy.Application.Models
{
    public class BuyerZipCoverage
    {
        public int BuyerZipCoverageId { get; set; }
        public int BuyerId { get; set; }
        public string ZipCode { get; set; } = null!;
        public decimal DefaultQuoteAmount { get; set; }

        public Buyer Buyer { get; set; } = null!;
        public ZipCode Zip { get; set; } = null!;
        public ICollection<OrderBuyerQuote> OrderBuyerQuotes { get; set; } = new List<OrderBuyerQuote>();
    }
}
