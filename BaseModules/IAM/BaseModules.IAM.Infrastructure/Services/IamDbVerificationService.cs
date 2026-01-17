using Arfware.ArfBlocks.Core;
using Arfware.ArfBlocks.Core.Exceptions;
using Common.Definitions.Domain.Errors;
using Common.Services.Auth.Authorization;
using Common.Services.ErrorCodeGenerator;


namespace BaseModules.IAM.Infrastructure.Services;

public class IamDbVerificationService
{
	private readonly IamDbContext _dbContext;
	private readonly AuthorizationService _authorizationService;
	private readonly CurrentUserService _currentUserService;
	private readonly EnvironmentService _environmentService;

	public IamDbVerificationService(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
		_authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
		_environmentService = dependencyProvider.GetInstance<EnvironmentService>();
	}

	public async Task VerifyUserCanLogin(string userEmail)
	{
		// Get
		var user = await _dbContext.AppUsers.Where(u => !u.IsDeleted && u.Email.ToLower() == userEmail.ToLowerCase()).FirstOrDefaultAsync();

		// Check
		if (!user.IsActive)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserBlocked));
	}
	//KoopCard
	public void VerifyUsersSelfCall(Guid userId, Guid currentUserId)
	{
		if (userId != currentUserId)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserCanOnlyMakeActionsForSelf));
	}


	//Login
	public async Task VerifyUserCanLogin(Guid userId)
	{
		// Get
		var user = await _dbContext.AppUsers.Where(u => u.Id == userId).FirstOrDefaultAsync();

		// Check
		if (user.IsDeleted)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserDeleted));
		if (!user.IsActive)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserBlocked));
		// if (user.IsBanned)
		// 	throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserBanned));
	}


	public async Task VerifyUserCanDeleteAccount(string userEmail)
	{
		var currentUserId = _currentUserService.GetCurrentUserId();
		var currentUser = await _dbContext.AppUsers.Where(u => u.Id == currentUserId).FirstOrDefaultAsync();

		// Check
		if (currentUser.Email != userEmail)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserCanDeleteOnlyOwnAccount));

		// Check
		if (currentUser.UserName != "teknik")
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.AdAccountsCanBeDeletedByAdministrator));
	}



	public void VerifyCanBeUpdateProxyEndDateForDateTimeNow(DateTime endDate)
	{
		var dateTime = DateTime.UtcNow;
		if (dateTime > endDate)
			throw new ArfBlocksVerificationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserProxyErrors.EndDateCantBeSmallerThanThisHour));
	}

	public void VerifyCanBeUpdateProxyEndDateForStartDate(DateTime endDate, DateTime startDate)
	{
		if (startDate > endDate)
			throw new ArfBlocksVerificationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserProxyErrors.EndDateCantBeSmallerThanStartDate));
	}


	public async Task VerifyEnvironment()
	{
		if (_environmentService.Environment == CustomEnvironments.Production)
			throw new ArfBlocksVerificationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.VerificationEnvironmentInvalidFormat));

	}
}
