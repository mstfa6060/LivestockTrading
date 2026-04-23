using System;
using System.Reflection;
using System.Threading.Tasks;
using Arfware.ArfBlocks.Core;
using Arfware.ArfBlocks.Core.Models;
using Common.Definitions.Base.Enums;
using Common.Services.Auth.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseModules.FileProvider.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class BucketsController : ControllerBase
{

    private readonly AuditLogService _auditLogService;
    private readonly ArfBlocksRequestOperator _requestOperator;

    public BucketsController(ArfBlocksDependencyProvider dependencyBuilder)
    {
        _auditLogService = new AuditLogService(dependencyBuilder);
        _requestOperator = new ArfBlocksRequestOperator(dependencyBuilder);
    }


    /// <summary>
    /// Get a File
    /// </summary>
    /// <response code="200">Returns File Bucket in Response.Payload.</response>
    [HttpPost("[action]")]
    public async Task<ActionResult<Application.RequestHandlers.Buckets.Queries.Detail.ResponseModel>> Detail([FromBody] Application.RequestHandlers.Buckets.Queries.Detail.RequestModel payload)
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
        var result = await _requestOperator.OperateInternalRequest<Application.RequestHandlers.Buckets.Queries.Detail.Handler>(payload);
        await _auditLogService.CreateLogPhaseTwo(endpoint, result, AllModules.FileProvider, requestId);

        return new OkObjectResult(result);
    }


    /// <summary>
    /// Duplicate a Bucket
    /// </summary>
    /// <response code="200">Returns Duplicated Bucket Meta-Data in Response.Payload</response>
    [HttpPost("[action]")]
    public async Task<ActionResult<Application.RequestHandlers.Buckets.Commands.Duplicate.ResponseModel>> Duplicate([FromBody] Application.RequestHandlers.Buckets.Commands.Duplicate.RequestModel payload)
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
        var result = await _requestOperator.OperateInternalRequest<Application.RequestHandlers.Buckets.Commands.Duplicate.Handler>(payload);
        await _auditLogService.CreateLogPhaseTwo(endpoint, result, AllModules.FileProvider, requestId);

        return new OkObjectResult(result);
    }

    /// <summary>
    /// Copy a Bucket
    /// </summary>
    /// <response code="200">Returns Copied Bucket Meta-Data in Response.Payload</response>
    [HttpPost("[action]")]
    public async Task<ActionResult<Application.RequestHandlers.Buckets.Commands.Copy.ResponseModel>> Copy([FromBody] Application.RequestHandlers.Buckets.Commands.Copy.RequestModel payload)
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
        var result = await _requestOperator.OperateInternalRequest<Application.RequestHandlers.Buckets.Commands.Copy.Handler>(payload);
        await _auditLogService.CreateLogPhaseTwo(endpoint, result, AllModules.FileProvider, requestId);

        return new OkObjectResult(result);
    }


    // [HttpPost("[action]")]
    // public async Task Special()
    // {
    //     var requestModel = new Application.EventHandlers.Files.Commands.Approve.RequestModel()
    //     {
    //         BucketId = "664f22857f10db41ae06615a",
    //         UserId = Guid.Parse("03eb57e6-926d-4949-bf01-08dbd6226314"),
    //     };
    //     var result = await _requestOperator.OperateEvent<Application.EventHandlers.Files.Commands.Approve.Handler>(requestModel);

    //     if (result.StatusCode != 200)
    //     {
    //         System.Console.WriteLine($"FILE APPROVE EVENT ERROR: {result.Error?.Message}");
    //     }
    // }
}
