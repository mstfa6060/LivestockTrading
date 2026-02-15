namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Common.Definitions.Domain.Entities.User entity)
	{
		if (!string.IsNullOrWhiteSpace(request.FirstName))
			entity.FirstName = request.FirstName;

		if (!string.IsNullOrWhiteSpace(request.Surname))
			entity.Surname = request.Surname;

		// Telefon numarası değiştiğinde IsPhoneVerified false olmalı
		if (request.PhoneNumber != null)
		{
			if (entity.PhoneNumber != request.PhoneNumber)
				entity.IsPhoneVerified = false;
			entity.PhoneNumber = request.PhoneNumber;
		}

		if (request.CountryId.HasValue && request.CountryId.Value > 0)
			entity.CountryId = request.CountryId.Value;

		if (!string.IsNullOrWhiteSpace(request.Language))
			entity.Language = request.Language;

		if (request.PreferredCurrencyCode != null)
			entity.PreferredCurrencyCode = request.PreferredCurrencyCode;

		if (request.AvatarUrl != null)
			entity.BucketId = request.AvatarUrl;

		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(Common.Definitions.Domain.Entities.User user)
	{
		var currencyCode = user.PreferredCurrencyCode ?? user.Country?.DefaultCurrencyCode;
		var currencySymbol = user.Country?.DefaultCurrencySymbol;

		return new ResponseModel
		{
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			FirstName = user.FirstName,
			Surname = user.Surname,
			FullName = $"{user.FirstName} {user.Surname}".Trim(),
			PhoneNumber = user.PhoneNumber,
			IsPhoneVerified = user.IsPhoneVerified,
			IsActive = user.IsActive,
			CountryId = user.CountryId,
			CountryCode = user.Country?.Code,
			CountryName = user.Country?.Name,
			Language = user.Language,
			CurrencyCode = currencyCode,
			CurrencySymbol = currencySymbol,
			AvatarUrl = user.BucketId,
		};
	}
}
