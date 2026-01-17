using Arfware.ArfBlocks.Core.Exceptions;
using Common.Definitions.Domain.Entities;
using Common.Definitions.Domain.Errors;
using Common.Definitions.Infrastructure.Services;
using Common.Services.ErrorCodeGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BaseModules.IAM.Infrastructure.Services;

public class IamDbValidationService : DefinitionDbValidationService
{
    private readonly IamDbContext _dbContext;


    public IamDbValidationService(IamDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task ValidateCompanyByTaxNumberNotExist(string taxNumber)
    {
        var exists = await _dbContext.AppCompanies.AnyAsync(x => x.TaxNumber == taxNumber);
        if (exists)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CompanyErrors.TaxNumberAlreadyExists));
    }

    public async Task ValidateCompanyExist(Guid companyId)
    {
        var exists = await _dbContext.AppCompanies.AnyAsync(x => x.Id == companyId);
        if (!exists)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CompanyErrors.NotFound));
    }
    public async Task ValidateUserByIdExist(Guid userId)
    {
        var exists = await _dbContext.AppUsers.AnyAsync(x => x.Id == userId);
        if (!exists)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));
    }

    public async Task<User> GetUserById(Guid userId)
    {
        var user = await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));

        return user;
    }

    public async Task ValidateRefreshTokenExist(Guid refreshTokenId)
    {
        var exists = await _dbContext.AppRefreshTokens.AnyAsync(x => x.Id == refreshTokenId);
        if (!exists)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.RefreshTokenNotFound));
    }

    public async Task GetUserByRefreshTokenAsync(string hashedRefreshToken)
    {
        var token = await _dbContext.AppRefreshTokens
            .FirstOrDefaultAsync(x => x.Token == hashedRefreshToken);

        if (token == null)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.RefreshTokenInvalid)
            );

        if (token.ExpiresAt < DateTime.UtcNow)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.RefreshTokenExpired)
            );

    }

    public async Task ValidateUserPhoneExist(string phoneNumber)
    {
        var exists = await _dbContext.AppUsers.AnyAsync(x => x.PhoneNumber == phoneNumber);
        if (!exists)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));
    }

    public async Task ValidateUserOtpVerification(Guid userId, string phoneNumber, Guid companyId, string otpCode)
    {
        var user = await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId && x.PhoneNumber == phoneNumber && x.CompanyId == companyId);
        if (user == null)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserNotExist));

        if (user.LastOtpCode != otpCode)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserLoginCodeIsNotCorrect));

        if (user.LastOtpSentAt == null || (DateTime.UtcNow - user.LastOtpSentAt.Value).TotalMinutes > 5)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserLoginCodeIsExpired));
    }



}
