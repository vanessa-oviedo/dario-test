namespace Wheelzy.Application.Models
{
    public class Buyer
    {
        public int BuyerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<BuyerZipCoverage> BuyerZipCoverages { get; set; } = new List<BuyerZipCoverage>();
    }
}
