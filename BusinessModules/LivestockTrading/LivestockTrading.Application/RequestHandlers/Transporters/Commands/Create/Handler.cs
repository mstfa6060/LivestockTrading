using Common.Definitions.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Create;

public class Handler : IRequestHandler
{
	// LivestockTrading Module ID (seed data'dan)
	private static readonly Guid LivestockTradingModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
	// Transporter Role ID (roles.json'dan)
	private static readonly Guid TransporterRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000005");

	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		// Duplicate kontrolu
		var existing = await _dataAccessLayer.GetExistingByUserId(request.UserId, cancellationToken);
		if (existing != null)
		{
			return ArfBlocksResults.Success(mapper.MapToResponse(existing));
		}

		var entity = mapper.MapToEntity(request);
		await _dataAccessLayer.AddTransporter(entity);

		// Transporter rolunu ata (best-effort)
		try
		{
			var hasRole = await _dataAccessLayer.UserHasTransporterRole(
				request.UserId, LivestockTradingModuleId, TransporterRoleId);

			if (!hasRole)
			{
				var userRole = new UserRole
				{
					Id = Guid.NewGuid(),
					UserId = request.UserId,
					RoleId = TransporterRoleId,
					ModuleId = LivestockTradingModuleId,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,
					IsDeleted = false
				};
				await _dataAccessLayer.AddUserRole(userRole);
			}
		}
		catch (Exception) { }

		return ArfBlocksResults.Success(mapper.MapToResponse(entity));
	}
}
