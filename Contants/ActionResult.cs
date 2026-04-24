namespace RMS.Contants;

public readonly record struct ActionCustomResult
{
    public bool Status { get; init; }
    public string Message { get; init; }
    public object? Data { get; init; }
}