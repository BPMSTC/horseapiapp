namespace horseapispring26.Models.DTOs;

public class PagedResponseDto<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}
