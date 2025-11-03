using depensio.Application.Services;

namespace depensio.Application.UseCases.Auth.Commands.Refresh;


public class RefreshTokenHandler(
        IHttpContextAccessor _httpContextAccessor,
        AuthorizationService _authServices,
        IDepensioDbContext _dbContext,
        IUnitOfWork _unitOfWork,
        IGenericRepository<RefreshToken> _refreshTokenRepository
    )
    : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
        {
            return (RefreshTokenResult)Results.Unauthorized(); ;
        }

        var refreshTokenHash = AuthHelper.HashToken(refreshToken);

        var tokenEntity = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(r =>
                r.TokenHash == refreshTokenHash &&
                !r.IsRevoked &&
                r.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

        if (tokenEntity == null)
            return (RefreshTokenResult)Results.Unauthorized();

        var jwtToken = new JwtTokenModel
        {
            Email = tokenEntity.Email,
            UserId = tokenEntity.UserId,
            Role = "DashbordAdmin",
        };

        var result = await _authServices.GetTokenAsync(jwtToken);
        tokenEntity.IsRevoked = true;
        tokenEntity.RevokedReason = "Utilisé pour refresh";
        _refreshTokenRepository.UpdateData(tokenEntity);

        refreshTokenHash = AuthHelper.HashToken(result.RefreshToken);
        var newTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = refreshTokenHash,
            Email = tokenEntity.Email,
            UserId = tokenEntity.UserId,
            Role = tokenEntity.Role,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddDataAsync(newTokenEntity);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new RefreshTokenResult(result.Token);
    }
}
