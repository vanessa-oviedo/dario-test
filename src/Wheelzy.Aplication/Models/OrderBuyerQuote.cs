using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheelzy.Application.Models
{
    public class OrderBuyerQuote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderBuyerQuoteId { get; set; }
        public int OrderId { get; set; }
        public int BuyerZipCoverageId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Order Order { get; set; } = null!;
        public BuyerZipCoverage BuyerZipCoverage { get; set; } = null!;
    }
}
