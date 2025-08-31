namespace Wheelzy.Application.DTOs.Case;

public sealed class CreateSellCaseDto
{
    public int CustomerId { get; init; }
    public int CarId { get; init; }
    public string ZipCode { get; init; } = string.Empty;
    public DateTime? CreatedAtUtc { get; init; }
}
