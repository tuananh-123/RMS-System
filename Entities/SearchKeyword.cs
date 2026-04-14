namespace RMS.Entities
{
    public class SearchKeyword
    {
        public string[] Hashtags { get; set { if (value != null) value = [.. value.Select(h => h.Trim())]; } } = [];
        public string[] Keywords { get; set { if (value != null) value = [.. value.Select(kw => kw.Trim())]; } } = [];
    }
}