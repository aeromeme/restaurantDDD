namespace Domain.Results;
public class Result : IResult
{
    public bool Success { get; }
    public string Message { get; }

    public Result(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public static Result Ok(string message = "") => new Result(true, message);
    public static Result Fail(string message) => new Result(false, message);
}

public class Result<T> : Result, IResult<T>
{
    public T Data { get; }

    public Result(bool success, string message, T data)
        : base(success, message)
    {
        Data = data;
    }

    public static Result<T> Ok(T data, string message = "") => new Result<T>(true, message, data);
    public static Result<T> Fail(string message) => new Result<T>(false, message, default!);
}