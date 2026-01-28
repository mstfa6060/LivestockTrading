using Common.Definitions.Domain.Entities;

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Create;

/// <summary>
/// Kullanıcı Oluşturma
/// Bu endpoint, sistemde yeni bir kullanıcı hesabı oluşturur.
/// Kullanıcı bilgileri ve kimlik doğrulama ayarları kaydedilir.
/// Yeni kullanıcıya otomatik olarak LivestockTrading modülünde Buyer rolü atanır.
/// </summary>
public class Handler : IRequestHandler
{
	// LivestockTrading Module ID (from modules.json)
	private static readonly Guid LivestockTradingModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
	// Role IDs (from roles.json)
	private static readonly Guid AdminRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
	private static readonly Guid BuyerRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000006");

	// Admin email adresleri - bu kullanıcılar kayıt olduğunda otomatik Admin rolü alır
	private static readonly string[] AdminEmails = new[]
	{
		"nagehanyazici13@gmail.com",
		"m.mustafaocak@gmail.com"
	};

	private readonly DataAccess _dataAccessLayer;
	private readonly ArfBlocksCommunicator _communicator;
	private readonly EnvironmentService _environmentService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess) //  `object` yerine `DataAccess` kullanıldı
	{
		_dataAccessLayer = dataAccess;
		_communicator = dependencyProvider.GetInstance<ArfBlocksCommunicator>();
		_environmentService = dependencyProvider.GetInstance<EnvironmentService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var requestPayload = (RequestModel)payload;

		string hashedPassword = null;
		string salt = null;

		if (requestPayload.UserSource == UserSources.Manual)
		{
			// Şifre hashlemesi yap
			salt = SecurityHelper.GenerateSalt();
			hashedPassword = SecurityHelper.HashPassword(requestPayload.Password, salt);
		}

		// Kullanıcı nesnesini oluştur
		var user = mapper.MapToNewEntity(requestPayload, hashedPassword, salt);

		// Veritabanına ekle
		await _dataAccessLayer.AddUser(user);

		// Email adresine göre rol belirle (Admin veya Buyer)
		var isAdminEmail = AdminEmails.Contains(user.Email, StringComparer.OrdinalIgnoreCase);
		var roleId = isAdminEmail ? AdminRoleId : BuyerRoleId;

		// Kullanıcıya rol ata
		var userRole = new UserRole
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			RoleId = roleId,
			ModuleId = LivestockTradingModuleId,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow,
			IsDeleted = false
		};
		await _dataAccessLayer.AddUserRole(userRole);

		// Country bilgisi ile birlikte tekrar çek
		var userWithCountry = await _dataAccessLayer.GetUserWithCountry(user.Id);

		var response = mapper.MapToResponse(userWithCountry);
		return ArfBlocksResults.Success(response);
	}
}
