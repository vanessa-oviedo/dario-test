namespace Wheelzy.Infrastructure.Persistence
{
    public class BuyerZipRate
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
