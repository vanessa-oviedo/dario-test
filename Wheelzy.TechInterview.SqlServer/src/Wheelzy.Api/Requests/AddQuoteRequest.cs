using System.ComponentModel.DataAnnotations;

public record AddQuoteRequest([Range(1, 1_000_000)] decimal Amount, int BuyerId);
