namespace UserService.Models.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public string? Error { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> Fail(string error, string? message = null) => new()
    {
        Success = false,
        Error = error,
        Message = message
    };
}
