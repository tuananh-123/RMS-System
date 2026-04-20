namespace RMS.Dtos.Recipes;

public readonly record struct RecipePagingResponseDto
{
    public List<RecipeDto> Rows { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}