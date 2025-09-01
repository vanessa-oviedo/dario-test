namespace Wheelzy.Application.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    public int CustomerId { get; set; }
    public int? OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? DueAt { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Currency { get; set; } = "USD";
    public string? ExternalNumber { get; set; }
    public string? Notes { get; set; }

    public Customer Customer { get; set; } = null!;
    public Order? Order { get; set; }
}