namespace Wheelzy.Application.DTOs.Common;

public sealed class PageRequest
{
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 50;
}
