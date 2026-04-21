namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.VerifyEmailOtp;

/// <summary>
/// E-posta OTP Dogrulama
/// Kullanicinin girdigi OTP kodunu dogrular ve e-postayi onaylanmis olarak isaretler.
/// </summary>
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccess;

    public Handler(ArfBlocksDependencyProvider dp, object dataAccess)
    {
        _dataAccess = (DataAccess)dataAccess;
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel model, EndpointContext ctx, CancellationToken ct)
    {
        var request = (RequestModel)model;

        var user = await _dataAccess.GetUserById(request.UserId)
                   ?? await _dataAccess.GetUserByEmail(request.Email);

        if (user == null)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));

        if (user.LastOtpCode != request.OtpCode)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserOtpCodeInvalid));

        // OTP 10 dakika gecerli
        if (user.LastOtpSentAt.HasValue && DateTime.UtcNow - user.LastOtpSentAt.Value > TimeSpan.FromMinutes(10))
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserOtpCodeExpired));

        user.EmailConfirmed = true;
        user.LastOtpVerifiedAt = DateTime.UtcNow;
        user.LastOtpCode = null;

        await _dataAccess.SaveChanges();

        return ArfBlocksResults.Success(new ResponseModel { IsSuccess = true });
    }
}
