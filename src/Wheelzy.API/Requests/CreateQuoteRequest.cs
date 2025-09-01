using System.ComponentModel.DataAnnotations;

namespace Wheelzy.API.Requests
{
    public class CreateQuoteRequest
    {
        [Required]
        public int? OrderId { get; set; }

        [Required]
        public int? BuyerId { get; set; }

        [Required]
        public decimal? AmountOverride { get; set; }
    }
}
