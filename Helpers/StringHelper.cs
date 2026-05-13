using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RMS.Helpers;

public static class StringHelper
{
    public static string ToSlug(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        string slug = input.ToLowerInvariant();

        slug = input.Normalize(NormalizationForm.FormD);
        slug = new string([.. input.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)]);

        slug = slug.Trim().Replace(" ", "-");

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
}