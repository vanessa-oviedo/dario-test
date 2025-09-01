namespace Wheelzy.Application.DTOs.Order;

public sealed class CreateSellCaseDto
{
    public bool SetInitialStatus;

    public int CustomerId { get; init; }
    public int CarId { get; init; }
    public string ZipCode { get; init; } = string.Empty;
    public DateTime? CreatedAtUtc { get; init; }
}
