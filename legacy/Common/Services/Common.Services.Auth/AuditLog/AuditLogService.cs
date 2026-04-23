using Arfware.ArfBlocks.Core;
using Arfware.ArfBlocks.Core.Exceptions;
using Common.Definitions.Base.Entity;
using Common.Definitions.Domain.Entities;
using Common.Definitions.Base.Enums;
using Common.Definitions.Infrastructure.RelationalDB;
using Common.Services.Auth.CurrentUser;
using Microsoft.EntityFrameworkCore;
using Common.Services.Environment;
using Arfware.ArfBlocks.Core.Models;
using Arfware.ArfBlocks.Core.Abstractions;
using Arfware.ArfBlocks.Core.Contexts;
using System.Text.Json;

namespace Common.Services.Auth.Authorization;

public class AuditLogService
{
	private readonly DefinitionDbContext _dbContext;
	private readonly DbContext _commonDbContext;

	private readonly CurrentUserService _currentUserService;
	private Resource resource;
	private readonly EnvironmentService _environmentService;
	private readonly JsonSerializerOptions _jsonSerializerOptions;

	public AuditLogService(ArfBlocksDependencyProvider dependencyProvider)
	{
		_commonDbContext = dependencyProvider.GetInstance<DbContext>();
		_dbContext = (DefinitionDbContext)_commonDbContext;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
		_environmentService = dependencyProvider.GetInstance<EnvironmentService>();
		_jsonSerializerOptions = new JsonSerializerOptions()
		{
			WriteIndented = false,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};
	}

	// TODO: Change moduletypes as allmodules enum
	public async Task<bool> CreateLog(EndpointModel endpoint, IRequestModel payload, ArfBlocksRequestResult result, EndpointContext context, AllModules moduleType, string _requestId = null)
	{
		// Skip log creation
		return true;

		try
		{

			var requestId = _requestId != null ? _requestId : context.GetParentRequestId();
			var auditLog = await _dbContext.AppAuditLogs.FirstOrDefaultAsync(a => a.RequestId == requestId);

			if (auditLog == null)
			{
				(var currentUserId, var currentUserIp) = GetCurrentUserInfoSafely();

				auditLog = new AuditLog()
				{
					// TODO: Change property as string and remove guid parsing
					RequestId = requestId,
					UserId = currentUserId,
					UserIp = currentUserIp,
					ModuleType = moduleType,

					ActionName = endpoint.ActionName,
					ObjectName = endpoint.ObjectName,
					IsAllowAnonymous = endpoint.IsAllowAnonymous,
					IsAuthorize = endpoint.IsAuthorize,
					IsInternal = endpoint.IsInternal,
					IsEventHandler = endpoint.IsEventHandler,
					EndpointType = endpoint.EndpointType == EndpointModel.EndpointTypes.Command ? EndpointTypes.Command : EndpointTypes.Query,

					RequestPayloadAsJson = JsonSerializer.Serialize(payload as object, _jsonSerializerOptions),
					HeadersAsJson = "",
					ListFilterAsJson = "",
					ListPageAsJson = "",
					ListSortingAsJson = "",
				};

				_dbContext.AppAuditLogs.Add(auditLog);
				await _dbContext.SaveChangesAsync();
			}
			else
			{
				auditLog.StatusCode = result?.StatusCode ?? 0;
				auditLog.ErrorCode = result.HasError ? result?.Error?.Message : null;
				auditLog.HasError = result.HasError;
				auditLog.Code = result?.Code;
				auditLog.DurationMs = result?.DurationMs ?? 0;
				auditLog.IsResponseModelArray = result?.IsPayloadArray ?? false;
				auditLog.ResponsePayloadAsJson = JsonSerializer.Serialize(result?.Payload, _jsonSerializerOptions);

				_dbContext.AppAuditLogs.Update(auditLog);
				await _dbContext.SaveChangesAsync();
			}
		}
		catch (System.Exception ex)
		{
			System.Console.WriteLine("AuditLog saving failed");
			System.Console.WriteLine(ex.Message);
			System.Console.WriteLine(ex.StackTrace);
			System.Console.WriteLine(ex.InnerException?.Message);
		}

		return false;
	}

	public async Task<bool> CreateLogPhaseOne(EndpointModel endpoint, IRequestModel payload, AllModules moduleType, string requestId)
	{
		var context = new EndpointContext();

		return await this.CreateLog(endpoint, payload, null, context, moduleType, requestId);
	}


	public async Task<bool> CreateLogPhaseTwo(EndpointModel endpoint, ArfBlocksRequestResult result, AllModules moduleType, string requestId)
	{
		var context = new EndpointContext();

		return await this.CreateLog(endpoint, null, result, context, moduleType, requestId);
	}

	private (Guid? CurrentUserId, string CurrentUserIp) GetCurrentUserInfoSafely()
	{
		try
		{
			var currentUserId = _currentUserService.GetCurrentUserId();
			var currentUserIp = _currentUserService.GetCurrentUserIp();

			return (currentUserId, currentUserIp.ToString());
		}
		catch
		{
			return (null, null);
		}
	}

}

