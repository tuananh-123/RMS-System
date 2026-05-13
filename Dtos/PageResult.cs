namespace RMS.Dtos;

public readonly struct PageResult<T>(List<T>? items, int totalCount, int pageNumber, int pageSize, int totalPages) where T : struct
{
    public List<T>? Items { get; init; } = items;
    public int TotalCount { get; init; } = totalCount;
    public int PageNumber { get; init; } = pageNumber;
    public int PageSize { get; init; } = pageSize;
    public int TotalPages { get; init; } = totalPages;
}