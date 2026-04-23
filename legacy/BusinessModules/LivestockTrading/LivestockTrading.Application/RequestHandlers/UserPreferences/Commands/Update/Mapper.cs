using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Domain.Entities.UserPreferences entity)
	{
		entity.UserId = request.UserId;
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
