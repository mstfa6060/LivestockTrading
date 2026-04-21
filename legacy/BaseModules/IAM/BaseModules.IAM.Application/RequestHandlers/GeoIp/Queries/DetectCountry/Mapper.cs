namespace BaseModules.IAM.Application.RequestHandlers.GeoIp.Queries.DetectCountry;

public class Mapper
{
    public ResponseModel MapToResponse(Country country, string detectedCode)
    {
        if (country == null)
        {
            return new ResponseModel
            {
                CountryCode = detectedCode,
                CountryId = null,
                CountryName = null
            };
        }

        return new ResponseModel
        {
            CountryCode = country.Code,
            CountryId = country.Id,
            CountryName = country.Name
        };
    }
}
