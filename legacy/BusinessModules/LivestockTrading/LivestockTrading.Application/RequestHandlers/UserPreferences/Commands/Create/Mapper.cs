using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Commands.Create;

public class Mapper
{
	public Domain.Entities.UserPreferences MapToEntity(RequestModel request)
	{
		return new Domain.Entities.UserPreferences
		{
			Id = Guid.NewGuid(),
			UserId = request.UserId,
			PreferredCurrency = request.PreferredCurrency,
			PreferredLanguage = request.PreferredLanguage,
			CountryCode = request.CountryCode,
			TimeZone = request.TimeZone,
			WeightSystem = (MeasurementSystem)request.WeightSystem,
			DistanceSystem = (MeasurementSystem)request.DistanceSystem,
			AreaSystem = (MeasurementSystem)request.AreaSystem,
			EmailNotificationsEnabled = request.EmailNotificationsEnabled,
			SmsNotificationsEnabled = request.SmsNotificationsEnabled,
			PushNotificationsEnabled = request.PushNotificationsEnabled,
			DarkModeEnabled = request.DarkModeEnabled,
			ProductsPerPage = request.ProductsPerPage,
			DefaultViewMode = (ViewMode)request.DefaultViewMode,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
