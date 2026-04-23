using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Common.Definitions.Domain.NonRelational.Entities;
using Arfware.ArfBlocks.Core;
using Common.Services.Auth.Authorization;
using Arfware.ArfBlocks.Core.Contexts;
using Common.Definitions.Base.Enums;
using Arfware.ArfBlocks.Core.Models;
using System.Reflection;

namespace BaseModules.FileProvider.Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class FilesController : ControllerBase
	{
		private readonly AuditLogService _auditLogService;
		private readonly ArfBlocksRequestOperator _requestOperator;

		public FilesController(ArfBlocksDependencyProvider dependencyBuilder)
		{
			_auditLogService = new AuditLogService(dependencyBuilder);
			_requestOperator = new ArfBlocksRequestOperator(dependencyBuilder);
		}


		/// <summary>
		/// Upload a File
		/// </summary>
		/// <response code="200">Returns File Bucket in Response.Payload</response>
		[HttpPost("[action]")]
		[RequestSizeLimit(75_000_000)]
		public async Task<ActionResult<Application.RequestHandlers.Files.Commands.Upload.ResponseModel>> Upload([FromForm] string bucketId, [FromForm] Guid changeId, [FromForm] string moduleName, [FromForm] int bucketType, [FromForm] string folderName, [FromForm] Guid? entityId, [FromForm] string versionName, [FromForm] Guid? tenantId)
		{
			// Content-Type kontrolü
			if (!Request.HasFormContentType)
			{
				return BadRequest(new { Error = "Content-Type must be multipart/form-data for file uploads" });
			}

			var files = Request.Form.Files;
			var file = files != null && files.Count > 0 ? files[0] : null;
			var payload = new Application.RequestHandlers.Files.Commands.Upload.RequestModel()
			{
				BucketId = bucketId,
				FormFile = file,
				ChangeId = changeId,
				ModuleName = moduleName,
				FolderName = folderName,
				EntityId = entityId,
				VersionName = versionName,
				BucketType = (BucketTypes)bucketType,
				TenantId = tenantId
			};



			var endpoint = new Arfware.ArfBlocks.Core.Models.EndpointModel()
			{
				ActionName = MethodBase.GetCurrentMethod().Name,
				ObjectName = GetType().Name.Replace("Controller", ""),
				Context = null,
				DataAccess = null,
				EndpointType = EndpointModel.EndpointTypes.Query,
				Handler = null,
				IsAllowAnonymous = false,
				IsAuthorize = false,
				IsEventHandler = false,
				IsInternal = true,
				IsResponseModelArray = false,
				PostHandler = null,
				PreHandler = null,
				RequestModel = null,
				ResponseModel = null,
				Validator = null,
				Verificator = null
			};

			var requestId = Guid.NewGuid().ToString();
			await _auditLogService.CreateLogPhaseOne(endpoint, payload, AllModules.FileProvider, requestId);
			var result = await _requestOperator.OperateInternalRequest<Application.RequestHandlers.Files.Commands.Upload.Handler>(payload);
			await _auditLogService.CreateLogPhaseTwo(endpoint, result, AllModules.FileProvider, requestId);

			return new OkObjectResult(result);
		}




		/// <summary>
		/// Upload a File
		/// </summary>
		/// <response code="200">Returns File Bucket in Response.Payload</response>
		[HttpPost("[action]")]
		[RequestSizeLimit(100_000_000)]
		public async Task<ActionResult<Application.RequestHandlers.Files.Commands.UploadToApprovedBucket.ResponseModel>> UploadToApprovedBucket([FromForm] string bucketId, [FromForm] Guid changeId, [FromForm] string moduleName, [FromForm] int bucketType, [FromForm] string folderName, [FromForm] Guid? entityId, [FromForm] string versionName)
		{
			// Content-Type kontrolü
			if (!Request.HasFormContentType)
			{
				return BadRequest(new { Error = "Content-Type must be multipart/form-data for file uploads" });
			}

			var files = Request.Form.Files;
			var file = files != null && files.Count > 0 ? files[0] : null;
			var payload = new Application.RequestHandlers.Files.Commands.UploadToApprovedBucket.RequestModel()
			{
				BucketId = bucketId,
				FormFile = file,
				ChangeId = changeId,
				ModuleName = moduleName,
				FolderName = folderName,
				EntityId = entityId,
				VersionName = versionName,
				BucketType = (BucketTypes)bucketType
			};

			var endpoint = new Arfware.ArfBlocks.Core.Models.EndpointModel()
			{
				ActionName = MethodBase.GetCurrentMethod().Name,
				ObjectName = GetType().Name.Replace("Controller", ""),
				Context = null,
				DataAccess = null,
				EndpointType = EndpointModel.EndpointTypes.Query,
				Handler = null,
				IsAllowAnonymous = false,
				IsAuthorize = false,
				IsEventHandler = false,
				IsInternal = true,
				IsResponseModelArray = false,
				PostHandler = null,
				PreHandler = null,
				RequestModel = null,
				ResponseModel = null,
				Validator = null,
				Verificator = null
			};

			var requestId = Guid.NewGuid().ToString();
			await _auditLogService.CreateLogPhaseOne(endpoint, payload, AllModules.FileProvider, requestId);
			var result = await _requestOperator.OperateInternalRequest<Application.RequestHandlers.Files.Commands.UploadToApprovedBucket.Handler>(payload);
			await _auditLogService.CreateLogPhaseTwo(endpoint, result, AllModules.FileProvider, requestId);

			return new OkObjectResult(result);
		}

		/// <summary>
		/// Upload a File
		/// </summary>
		/// <response code="200">Returns File Bucket in Response.Payload</response>
		// Only used for migrations
		[HttpPost("[action]")]
		[RequestSizeLimit(500_000_000)]
		public async Task<ActionResult<Application.RequestHandlers.Files.Commands.UploadEyp.ResponseModel>> UploadEypOrPdf([FromForm] Guid entityId, [FromForm] string fileExtention)
		{
			// Content-Type kontrolü
			if (!Request.HasFormContentType)
			{
				return BadRequest(new { Error = "Content-Type must be multipart/form-data for file uploads" });
			}

			var files = Request.Form.Files;
			var file = files != null && files.Count > 0 ? files[0] : null;
			var payload = new Application.RequestHandlers.Files.Commands.UploadEyp.RequestModel()
			{
				FormFile = file,
				EntityId = entityId,
				FileExtention = fileExtention,
			};

			return await _requestOperator.OperateHttpRequest<Application.RequestHandlers.Files.Commands.UploadEyp.Handler>(payload);
		}


		/// <summary>
		/// Delete a File
		/// </summary>
		/// <response code="200">Returns File BucketId and File EntryId in Response.Payload</response>
		[HttpPost("[action]")]
		public async Task<ActionResult<Application.RequestHandlers.Files.Commands.Delete.ResponseModel>> Delete([FromBody] Application.RequestHandlers.Files.Commands.Delete.RequestModel payload)
		{

			var endpoint = new Arfware.ArfBlocks.Core.Models.EndpointModel()
			{
				ActionName = MethodBase.GetCurrentMethod().Name,
				ObjectName = GetType().Name.Replace("Controller", ""),
				Context = null,
				DataAccess = null,
				EndpointType = EndpointModel.EndpointTypes.Query,
				Handler = null,
				IsAllowAnonymous = false,
				IsAuthorize = false,
				IsEventHandler = false,
				IsInternal = true,
				IsResponseModelArray = false,
				PostHandler = null,
				PreHandler = null,
				RequestModel = null,
				ResponseModel = null,
				Validator = null,
				Verificator = null
			};

			var requestId = Guid.NewGuid().ToString();
			await _auditLogService.CreateLogPhaseOne(endpoint, payload, AllModules.FileProvider, requestId);
			var result = await _requestOperator.OperateInternalRequest<Application.RequestHandlers.Files.Commands.Delete.Handler>(payload);
			await _auditLogService.CreateLogPhaseTwo(endpoint, result, AllModules.FileProvider, requestId);

			return new OkObjectResult(result);
		}
	}
}
