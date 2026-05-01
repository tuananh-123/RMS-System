using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RMS.Helpers;

public static class StringHelper
{
    public static string ToSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        string slug = text.ToLowerInvariant();

        slug = RemoveDiacritics(slug);
        
        // 3. Thay thế các ký tự không phải chữ cái/số thành dấu gạch ngang
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // 4. Thay thế nhiều khoảng trắng/gạch ngang liên tiếp thành 1 gạch ngang
        slug = Regex.Replace(slug, @"[\s-]+", "-").Trim();

        slug = slug.Trim('-');

        return slug;
    }

    public static bool IsStrongPassword(this string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;

        var length = password.Length;
        if (length < 8) return false;
        if (!Regex.IsMatch(password, @"[A-Z]")) return false;
        if (!Regex.IsMatch(password, @"[a-z]")) return false;
        if (!Regex.IsMatch(password, @"[0-9]")) return false;
        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]")) return false;

        return true;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}