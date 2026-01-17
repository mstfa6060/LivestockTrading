using BaseModules.IAM.Infrastructure.RelationalDB;
using Common.Definitions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseModules.IAM.Application.EventHandlers.Sms.Queries.SendOtpSms;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<Common.Definitions.Domain.Entities.User> GetUserByPhone(string phoneNumber)
    {
        return await _dbContext.AppUsers
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }
}
