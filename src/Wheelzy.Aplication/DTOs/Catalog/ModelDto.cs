namespace Wheelzy.Application.DTOs.Catalog;

public sealed class ModelDto
{
    public int Id { get; init; }
    public int MakeId { get; init; }
    public string Name { get; init; } = string.Empty;
}
