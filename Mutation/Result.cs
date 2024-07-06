namespace Mutation;

public class Result
{
    protected Result(bool success, string error)
    {
        if (success && error != string.Empty || !success && error == string.Empty)
            throw new InvalidOperationException();

        IsSuccess = success;
        Error = error;
    }

    public bool IsSuccess { get; }
    public string Error { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Fail(string message)
    {
        return new Result(false, message);
    }

    public static Result<T> Fail<T>(string message)
    {
        T? t = default;
#pragma warning disable CS8604 // Existence possible d'un argument de référence null.
        return new Result<T>(t, false, message);
#pragma warning restore CS8604 // Existence possible d'un argument de référence null.
    }

    public static Result Ok()
    {
        return new Result(true, string.Empty);
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }
}

public class Result<T> : Result
{
    protected internal Result(T value, bool success, string error)
        : base(success, error)
    {
        Value = value;
    }

    public T Value { get; set; }
}