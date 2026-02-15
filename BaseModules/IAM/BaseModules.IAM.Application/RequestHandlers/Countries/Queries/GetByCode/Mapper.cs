namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.GetByCode;

public class Mapper
{
    public ResponseModel MapToResponse(Country entity)
    {
        return new ResponseModel
        {
            Id = entity.Id,
            Code = entity.Code,
            Code3 = entity.Code3,
            NumericCode = entity.NumericCode,
            Name = entity.Name,
            NativeName = entity.NativeName,
            Capital = entity.Capital,
            Continent = entity.Continent,
            Region = entity.Region,
            PhoneCode = entity.PhoneCode,
            DefaultCurrencyCode = entity.DefaultCurrencyCode,
            DefaultCurrencySymbol = entity.DefaultCurrencySymbol,
            Flag = entity.Flag,
            Timezone = entity.Timezone,
            DefaultLanguage = entity.DefaultLanguage,
            IsActive = entity.IsActive
        };
    }
}
