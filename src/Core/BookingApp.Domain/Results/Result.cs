namespace BookingApp.Domain.Results;

public class Result<T>
{
    private readonly T? _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException();
            }

            return _value!;
        }
    }

    public Error? Error { get; }

    private Result(T value)
    {
        _value = value;
        IsSuccess = true;
        Error = Error.None;
    }

    private Result(Error error)
    {
        if (error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Fail(Error error) => new(error);
}