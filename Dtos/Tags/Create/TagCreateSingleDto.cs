namespace RMS.Dtos.Tags.Create;

public readonly record struct TagCreateSingleDto
{
    public string Title { get; init; }
    public string PropertyColor { get; init; }
}