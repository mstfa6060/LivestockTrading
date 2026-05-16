namespace Shared.Results;

/// <summary>Değer taşıyan sonuç. Failure'da Value erişimi guard'lı (sessiz null yok).</summary>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value of a failure result.");

    internal Result(T? value, bool isSuccess, Error? error) : base(isSuccess, error)
        => _value = value;
}
