

namespace Service.Models;

public class Result
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? StatusCode { get; set; }
}

public class Result<T> : Result where T : class
{
    public T? Data { get; set; }
}
