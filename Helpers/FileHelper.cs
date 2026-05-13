namespace RMS.Helpers;

public static class FileHelper
{
    public static string GetExtension(this string fileName) => Path.GetExtension(fileName).ToLowerInvariant();

    public static async Task<string> ReadJsonFileContent(this IFormFile file)
    {
        using Stream stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        string jsonContent = await reader.ReadToEndAsync();
        return jsonContent;
    }

}