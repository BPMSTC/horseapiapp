namespace horseapispring26.Models;

public class PagedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
