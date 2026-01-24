using Arfware.ArfBlocks.Core;
using Arfware.ArfBlocks.Core.Exceptions;
using Common.Definitions.Infrastructure.Services;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Infrastructure.RelationalDB;
using Common.Services.ErrorCodeGenerator;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Infrastructure.Services;

public class LivestockTradingModuleDbValidationService : DefinitionDbValidationService
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public LivestockTradingModuleDbValidationService(ArfBlocksDependencyProvider dp) : base(dp.GetInstance<LivestockTradingModuleDbContext>())
	{
		_dbContext = dp.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task ValidateCategoryExist(Guid categoryId, CancellationToken ct = default)
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

	public async Task ValidateCategorySlugUnique(string slug, Guid? excludeId = null, CancellationToken ct = default)
	{
		var query = _dbContext.Categories
			.AsNoTracking()
			.Where(c => c.Slug == slug && !c.IsDeleted);

		if (excludeId.HasValue)
			query = query.Where(c => c.Id != excludeId.Value);

		var exists = await query.AnyAsync(ct);

		if (exists)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.SlugAlreadyExists));
		}
	}

	public async Task ValidateCategoryNameUnique(string name, Guid? parentCategoryId, Guid? excludeId = null, CancellationToken ct = default)
	{
		var query = _dbContext.Categories
			.AsNoTracking()
			.Where(c => c.Name == name && c.ParentCategoryId == parentCategoryId && !c.IsDeleted);

		if (excludeId.HasValue)
			query = query.Where(c => c.Id != excludeId.Value);

		var exists = await query.AnyAsync(ct);

		if (exists)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.NameAlreadyExists));
		}
	}

	public async Task ValidateParentCategoryExist(Guid? parentCategoryId, CancellationToken ct = default)
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

	public async Task ValidateCategoryHasNoChildren(Guid categoryId, CancellationToken ct = default)
	{
		var hasChildren = await _dbContext.Categories
			.AsNoTracking()
			.AnyAsync(c => c.ParentCategoryId == categoryId && !c.IsDeleted, ct);

		if (hasChildren)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.CategoryHasChildren));
		}
	}
}
