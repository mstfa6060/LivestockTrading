using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Domain.Entities.UserPreferences> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			UserId = e.UserId,
			PreferredCurrency = e.PreferredCurrency,
			PreferredLanguage = e.PreferredLanguage,
			CountryCode = e.CountryCode,
			TimeZone = e.TimeZone,
			WeightSystem = (int)e.WeightSystem,
			DistanceSystem = (int)e.DistanceSystem,
			AreaSystem = (int)e.AreaSystem,
			EmailNotificationsEnabled = e.EmailNotificationsEnabled,
			SmsNotificationsEnabled = e.SmsNotificationsEnabled,
			PushNotificationsEnabled = e.PushNotificationsEnabled,
			DarkModeEnabled = e.DarkModeEnabled,
			ProductsPerPage = e.ProductsPerPage,
			DefaultViewMode = (int)e.DefaultViewMode,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
