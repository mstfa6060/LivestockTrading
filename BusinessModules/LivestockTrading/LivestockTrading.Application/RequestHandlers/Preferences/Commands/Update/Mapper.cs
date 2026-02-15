using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Preferences.Commands.Update;

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
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};
	}

	public void MapToEntity(RequestModel request, Domain.Entities.UserPreferences entity)
	{
		entity.PreferredCurrency = request.PreferredCurrency;
		entity.PreferredLanguage = request.PreferredLanguage;
		entity.CountryCode = request.CountryCode;
		entity.TimeZone = request.TimeZone;
		entity.WeightSystem = (MeasurementSystem)request.WeightSystem;
		entity.DistanceSystem = (MeasurementSystem)request.DistanceSystem;
		entity.AreaSystem = (MeasurementSystem)request.AreaSystem;
		entity.EmailNotificationsEnabled = request.EmailNotificationsEnabled;
		entity.SmsNotificationsEnabled = request.SmsNotificationsEnabled;
		entity.PushNotificationsEnabled = request.PushNotificationsEnabled;
		entity.DarkModeEnabled = request.DarkModeEnabled;
		entity.ProductsPerPage = request.ProductsPerPage;
		entity.DefaultViewMode = (ViewMode)request.DefaultViewMode;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
