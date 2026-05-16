namespace Shared.Results;

/// <summary>
/// İşlem sonucu (void ops). IAdminCatalogCommands <c>Task&lt;Result&gt;</c> döner.
/// Static factory; success↔error tutarlılığı ctor'da zorlanır.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Success result cannot carry an error.");
        if (!isSuccess && error is null)
            throw new InvalidOperationException("Failure result requires an error.");
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Failure<T>(Error error) => new(default, false, error);
    public static Result<T> Failure<T>(string code, string message) => Failure<T>(new Error(code, message));
}
