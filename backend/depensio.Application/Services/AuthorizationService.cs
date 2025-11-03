

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
    public async Task<AccessToken> GetTokenAsync(JwtTokenModel jwtTokenModel)
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
            Expiration = DateTime.Now.AddMinutes(1)
        };
        var token = AuthHelper.GetTokenAsync(jwtToken);

        var httpContext = _httpContextAccessor.HttpContext;
        var refreshToken = AuthHelper.GenerateRefreshToken(
                httpContext: httpContext,
                expiration: DateTime.Now.AddDays(7)
            );
        return new AccessToken
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }

}
