public interface IResult
{
    bool Success { get; }
    string Message { get; }
}

public interface IResult<T> : IResult
{
    T Data { get; }
}