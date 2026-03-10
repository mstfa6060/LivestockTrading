using Common.Definitions.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Create;

public class Handler : IRequestHandler
{
	// LivestockTrading Module ID (seed data'dan)
	private static readonly Guid LivestockTradingModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
	// Seller Role ID (roles.json'dan)
	private static readonly Guid SellerRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000004");

	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var entity = mapper.MapToEntity(request);

		// Satıcı profilini oluştur
		await _dataAccessLayer.AddSeller(entity);

		// Kullanıcıya Seller rolünü ata (best-effort, satıcı oluşturmayı engellemez)
		try
		{
			var hasSellerRole = await _dataAccessLayer.UserHasSellerRole(
				request.UserId,
				LivestockTradingModuleId,
				SellerRoleId);

			if (!hasSellerRole)
			{
				var userRole = new UserRole
				{
					Id = Guid.NewGuid(),
					UserId = request.UserId,
					RoleId = SellerRoleId,
					ModuleId = LivestockTradingModuleId,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,
					IsDeleted = false
				};
				await _dataAccessLayer.AddUserRole(userRole);
			}
		}
		catch (Exception)
		{
			// Rol ataması başarısız olsa da satıcı profili oluşturulmuş durumda
		}

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
