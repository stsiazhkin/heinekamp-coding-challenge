namespace Heinekamp.CodingChallenge.FileApi.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public Exception? Exception { get; }

    protected Result(bool isSuccess, string? errorMessage, Exception? exception = null)
    {
        if(isSuccess && (!string.IsNullOrEmpty(errorMessage) || exception is not null))
            throw new ArgumentException("Success Result must not have an errorMessage or exception");
        if(!isSuccess && string.IsNullOrEmpty(errorMessage))
            throw new ArgumentException("Fail Result must have an errorMessage");

        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public static Result Success() => new Result(
        isSuccess: true, errorMessage: null, exception: null);
    public static Result Fail(
        string errorMessage, Exception? exception = null) => 
        new Result(isSuccess: false, errorMessage, exception);
}

public class Result<T> : Result
{
    private readonly T value;
    
    public T Value 
    { 
        get 
        {
            if(!IsSuccess)
                throw new ArgumentException("Failed Result does not have a valid Value");
            return value;
        }
    }
    
    private Result(bool isSuccess, T value, string? errorMessage, Exception? exception = null) 
        : base(isSuccess, errorMessage, exception)
    {
        this.value = value;
    }

    public static Result<T> Success(T value) => new (
        isSuccess: true, value, errorMessage: null, exception: null);
    public static Result<T> Fail(
        string errorMessage, Exception? exception = null, T value = default!) => 
        new (isSuccess: false, value, errorMessage, exception);
}