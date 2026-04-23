using System.Linq.Expressions;

namespace LivestockTrading.Application.Extensions;

/// <summary>
/// Entity doğrulama için extension methodlar
/// </summary>
public static class EntityExtensions
{
	/// <summary>
	/// Entity null ise ArfBlocksValidationException fırlatır
	/// </summary>
	public static T EnsureExists<T>(this T? entity) where T : class
	{
		if (entity == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		return entity;
	}

	/// <summary>
	/// Entity null ise özel hata mesajı ile ArfBlocksValidationException fırlatır
	/// </summary>
	public static T EnsureExists<T, TError>(this T? entity, Expression<Func<TError>> errorCodeSelector) where T : class
	{
		if (entity == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(errorCodeSelector));

		return entity;
	}
}
