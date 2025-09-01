namespace Wheelzy.Application.Models
{
    public class ZipCode
    {
        // PK column name in DB is also "ZipCode"
        public string ZipCodeId { get; set; } = null!; // we'll map to column ZipCode

        public ICollection<BuyerZipCoverage> BuyerCoverages { get; set; } = new List<BuyerZipCoverage>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
