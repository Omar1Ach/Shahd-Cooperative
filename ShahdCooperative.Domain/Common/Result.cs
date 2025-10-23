namespace ShahdCooperative.Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, T? value, string? error, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error, string? errorCode = null) => new(false, default, error, errorCode);
    public static Result<T> NotFound(string error = "Resource not found") => new(false, default, error, "NOT_FOUND");
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, string? error, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);
    public static Result NotFound(string error = "Resource not found") => new(false, error, "NOT_FOUND");
}
