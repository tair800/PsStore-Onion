using Microsoft.AspNetCore.Http;

public class Result<T>
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public int StatusCode { get; }
    public string ErrorCode { get; }
    public T Data { get; }

    private Result(bool isSuccess, string error, int statusCode, string errorCode, T data)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Data = data;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, string.Empty, StatusCodes.Status200OK, string.Empty, data);
    }

    public static Result<T> Failure(string error, int statusCode, string errorCode)
    {
        return new Result<T>(false, error, statusCode, errorCode, default);
    }
}
