//using Microsoft.AspNetCore.Http;

//public class Result
//{
//    public bool IsSuccess { get; }
//    public string Error { get; }
//    public int StatusCode { get; }
//    public string ErrorCode { get; }

//    private Result(bool isSuccess, string error, int statusCode, string errorCode)
//    {
//        IsSuccess = isSuccess;
//        Error = error;
//        StatusCode = statusCode;
//        ErrorCode = errorCode;
//    }

//    public static Result Success() => new Result(true, string.Empty, StatusCodes.Status200OK, string.Empty);
//    public static Result Failure(string error, int statusCode, string errorCode)
//        => new Result(false, error, statusCode, errorCode);
//}
