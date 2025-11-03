using depensio.Application.Services;
using IDR.Library.BuildingBlocks.Services;

namespace depensio.Application.UseCases.Auth.Commands.Refresh;


public class RefreshTokenHandler(
        IHttpContextAccessor _httpContextAccessor,
        AuthorizationService _authServices,
        IUnitOfWork _unitOfWork,
        RefreshTokenService<RefreshToken> _refreshTokenService
    )
    : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var remenberMe = request.RemenberMe;

        var httpContext = _httpContextAccessor.HttpContext;
        if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
        {
            return (RefreshTokenResult)Results.Unauthorized(); ;
        }

        var refreshTokenHash = AuthHelper.HashToken(refreshToken);

        var tokenEntity = await _refreshTokenService.GetRefreshTokenByHashAsync(refreshTokenHash, cancellationToken);

        if (tokenEntity == null)
            return (RefreshTokenResult)Results.Unauthorized();

        var jwtToken = new JwtTokenModel
        {
            Email = tokenEntity.Email,
            UserId = tokenEntity.UserId,
            Role = "DashbordAdmin",
        };

        var resultToken = await _authServices.GetTokenAsync(jwtToken, remenberMe);
        await _refreshTokenService.RevokeRefreshTokenAsync(refreshTokenHash, "Utilisé pour refresh", cancellationToken);

        await _refreshTokenService.RevokeAllUserTokensAsync(tokenEntity.UserId, "Utilisé pour refresh", cancellationToken);
        await _refreshTokenService.SaveRefreshTokenAsync(resultToken, jwtToken, cancellationToken);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new RefreshTokenResult(resultToken.Token);
    }
}
