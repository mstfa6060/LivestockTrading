using BaseModules.IAM.Infrastructure.RelationalDB;
using Common.Definitions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseModules.IAM.Application.EventHandlers.Mails.Queries.SendForgotPassword;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<Common.Definitions.Domain.Entities.User> GetByEmail(string email)
    {
        return await _dbContext.AppUsers
            .Where(x => x.Email.ToLower() == email.ToLower())
            .FirstOrDefaultAsync();
    }
}
