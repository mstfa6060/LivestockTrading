using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(Domain.Entities.UserPreferences entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			PreferredCurrency = entity.PreferredCurrency,
			PreferredLanguage = entity.PreferredLanguage,
			CountryCode = entity.CountryCode,
			TimeZone = entity.TimeZone,
			WeightSystem = (int)entity.WeightSystem,
			DistanceSystem = (int)entity.DistanceSystem,
			AreaSystem = (int)entity.AreaSystem,
			EmailNotificationsEnabled = entity.EmailNotificationsEnabled,
			SmsNotificationsEnabled = entity.SmsNotificationsEnabled,
			PushNotificationsEnabled = entity.PushNotificationsEnabled,
			DarkModeEnabled = entity.DarkModeEnabled,
			ProductsPerPage = entity.ProductsPerPage,
			DefaultViewMode = (int)entity.DefaultViewMode,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
