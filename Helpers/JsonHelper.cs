using System.Text.Json;

namespace RMS.Helpers;

public static class JsonHelper
{
    public static List<T> DeserializeToListObject<T>(this string jsonContent) where T : class =>
    JsonSerializer.Deserialize<List<T>>(jsonContent, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    })!;

}