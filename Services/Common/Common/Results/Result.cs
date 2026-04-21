namespace Common.Results;

public sealed record Result
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public List<Error> Errors { get; init; } = [];

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(string message, params Error[] errors) =>
        new() { IsSuccess = false, Message = message, Errors = [..errors] };

    public static Result<T> Success<T>(T data) =>
        new() { IsSuccess = true, Data = data };

    public static Result<T> Failure<T>(string message, params Error[] errors) =>
        new() { IsSuccess = false, Message = message, Errors = [..errors] };
}

public sealed record Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string Message { get; init; } = string.Empty;
    public List<Error> Errors { get; init; } = [];
}

public sealed record Error(string Code, string Message);
