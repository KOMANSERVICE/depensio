

namespace depensio.Application.Services;

public class AuthorizationService(
        IDepensioDbContext _dbContext,
        IConfiguration _configuration,
        ISecureSecretProvider _secureSecretProvider,
        IHttpContextAccessor _httpContextAccessor
    )
{

    public void GetUserPermissionForBoutique(string userId, Guid boutiqueId)
    {
        var authBoutique = _dbContext.Boutiques
                  .Any(b => b.Id == BoutiqueId.Of(boutiqueId)
                              && b.UsersBoutiques.Any(ub => ub.UserId == userId));
        if (!authBoutique)
        {
            throw new UnauthorizedException("You are not authorized to add barcode to this product");
        }
    }
    public async Task<AccessToken> GetTokenAsync(JwtTokenModel jwtTokenModel, bool remeberMe)
    {
        var JWT_Secret = _configuration["JWT:Secret"]!;
        var JWT_ValidIssuer = _configuration["JWT:ValidIssuer"]!;
        var JWT_ValidAudience = _configuration["JWT:ValidAudience"]!;

        var secret = await _secureSecretProvider.GetSecretAsync(JWT_Secret);
        //var issuer = await _secureSecretProvider.GetSecretAsync(JWT_ValidIssuer);
        //var audience = await _secureSecretProvider.GetSecretAsync(JWT_ValidAudience);
        var jwtToken = new JwtTokenModel
        {
            Email = jwtTokenModel.Email,
            UserId = jwtTokenModel.UserId,
            Roles = jwtTokenModel.Roles.ToList(),
            JwtSecret = secret,
            JwtIssuer = JWT_ValidIssuer,
            JwtAudience = JWT_ValidAudience,
            Expiration = DateTime.UtcNow.AddMinutes(5)
        };

        var refreshToken = new RefreshTokenModel
        {
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
            remeberMe = remeberMe
        };
        var httpContext = _httpContextAccessor.HttpContext;

        var accesToken = AuthHelper.GetAccessToken(httpContext, jwtToken, refreshToken);

        return new AccessToken
        {
            Token = accesToken.Token,
            RefreshToken = accesToken.RefreshToken,
            RefreshTokenHash = accesToken.RefreshTokenHash,
            RefreshTokenExpiration = accesToken.RefreshTokenExpiration
        };
    }

}
