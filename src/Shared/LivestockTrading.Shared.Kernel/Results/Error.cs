namespace Shared.Results;

/// <summary>Hata kimliği. Code = Madde 9 error-code matrix (string sabit), Message = insan-okur.</summary>
public sealed record Error(string Code, string Message);
