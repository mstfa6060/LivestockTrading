namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.ExportData;

public class RequestModel : IRequestModel
{
    public Guid UserId { get; set; }
}

public class ResponseModel : IResponseModel
{
    public UserDataExport Data { get; set; }
}

public class UserDataExport
{
    public ProfileData Profile { get; set; }
    public List<SellerData> Sellers { get; set; } = new();
    public List<ProductData> Products { get; set; } = new();
    public List<LocationData> Locations { get; set; } = new();
    public string ExportedAt { get; set; }
}

public class ProfileData
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
    public string Language { get; set; }
    public string CreatedAt { get; set; }
}

public class SellerData
{
    public string BusinessName { get; set; }
    public string BusinessType { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class ProductData
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public double BasePrice { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public string CreatedAt { get; set; }
}

public class LocationData
{
    public string Name { get; set; }
    public string City { get; set; }
    public string CountryCode { get; set; }
    public string AddressLine1 { get; set; }
}
