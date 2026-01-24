using Arfware.ArfBlocks.Core.Exceptions;
using Common.Definitions.Infrastructure.Services;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Infrastructure.RelationalDB;
using Common.Services.ErrorCodeGenerator;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Infrastructure.Services;

public class LivestockTradingModuleDbVerificationService : DefinitionDbValidationService
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public LivestockTradingModuleDbVerificationService(LivestockTradingModuleDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task ValidateCategoryExists(Guid categoryId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Categories
			.AsNoTracking()
			.AnyAsync(c => c.Id == categoryId && !c.IsDeleted, ct);

		if (!exists)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.CategoryNotFound));
		}
	}

	public async Task ValidateCategoryIsActive(Guid categoryId, CancellationToken ct = default)
	{
		var category = await _dbContext.Categories
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == categoryId && !c.IsDeleted, ct);

		if (category == null)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.CategoryNotFound));
		}

		if (!category.IsActive)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.CategoryNotActive));
		}
	}

	public async Task ValidateParentCategoryExists(Guid? parentCategoryId, CancellationToken ct = default)
	{
		if (parentCategoryId == null || parentCategoryId == Guid.Empty)
			return;

		var exists = await _dbContext.Categories
			.AsNoTracking()
			.AnyAsync(c => c.Id == parentCategoryId && !c.IsDeleted, ct);

		if (!exists)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.ParentCategoryNotFound));
		}
	}
}
