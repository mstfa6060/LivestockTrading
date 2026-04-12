namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.ExportData;

public class Mapper
{
    public UserDataExport MapToExport(Common.Definitions.Domain.Entities.User user)
    {
        return new UserDataExport
        {
            Profile = new ProfileData
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Language = user.Language,
                CreatedAt = user.CreatedAt.ToString("o")
            },
            ExportedAt = DateTime.UtcNow.ToString("o")
        };
    }
}
