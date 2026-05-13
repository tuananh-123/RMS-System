using System.Text.Json.Serialization;

namespace RMS.Dtos.Tags.Create;

public class TagCreateDto
{
    [JsonPropertyName("Title")]
    public string Title { get; init; } = string.Empty;
    [JsonPropertyName("Color")]
    public string PropertyColor { get; init; } = string.Empty;
    [JsonPropertyName("Color_Hex")]
    public string Color_Hex { get; set; } = string.Empty;

}