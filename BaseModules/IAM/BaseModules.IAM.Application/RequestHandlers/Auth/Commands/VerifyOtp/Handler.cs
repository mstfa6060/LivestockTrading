namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.VerifyOtp;

/// <summary>
/// OTP Kodu Doğrulama
/// Bu endpoint, kullanıcının girdiği OTP kodunu doğrular.
/// Başarılı doğrulamada telefon numarasını onaylanmış olarak işaretler.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccess;
	private readonly IJwtService _jwtService;

	public Handler(ArfBlocksDependencyProvider dp, object dataAccess)
	{
		_dataAccess = (DataAccess)dataAccess;
		_jwtService = dp.GetInstance<IJwtService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel model, EndpointContext ctx, CancellationToken ct)
	{
		var request = (RequestModel)model;
		var user = await _dataAccess.GetUserByPhoneAsync(request.PhoneNumber, request.CompanyId);

		user.IsPhoneVerified = true;
		user.LastOtpVerifiedAt = DateTime.UtcNow;
		await _dataAccess.UpdateUserAsync(user);

		var expiresAt = DateTime.UtcNow.AddDays(_jwtService.GetExpirationDayCount());


		var response = new ResponseModel
		{
			IsSuccess = true
		};

		return ArfBlocksResults.Success(response);
	}
}
