namespace Wheelzy.Application.DTOs.Common;

public sealed class PageResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Total { get; init; }
}
