namespace RMS.Contants;

public class ServiceResult(bool success, int code, string? message = null, object? data = null)
{
    public bool Success { get; set; } = success;
    public int Code { get; set; } = code;
    public string? Message { get; set; } = message;
    public object? Data { get; set; } = data;
}

