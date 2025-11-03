using depensio.Application.Interfaces;
using depensio.Application.UserCases.Auth.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace depensio.Application.UserCases.Auth.Queries.SignIn;

public class SignInHandler(
        SignInManager<ApplicationUser> _signInManager,
        IConfiguration _configuration,
        UserManager<ApplicationUser> _userManager,
        ISecureSecretProvider _secureSecretProvider,
        AuthorizationService _authServices,
        IGenericRepository<RefreshToken> _refreshTokenContext,
        IUnitOfWork _unitOfWork
    )    
    : IQueryHandler<SignInQuery, SignInResult>
{
    public async Task<SignInResult> Handle(SignInQuery query, CancellationToken cancellationToken)
    {
        var signIn = query.Signin;        

        var result = await _signInManager.PasswordSignInAsync(signIn.Email, signIn.Password, true, false);
        if (!result.Succeeded)
        {
            throw new BadRequestException($"Email ou mot de passe incorrect");
        }
        var user = await _userManager.FindByNameAsync(signIn.Email);

        if (user == null)
            throw new BadRequestException("Le mot de passe ou l'adresse email est incorrect.");

        var isConfirm = await _userManager.IsEmailConfirmedAsync(user);
        if (!isConfirm)
            throw new UnauthorizedException("Email non confirmé. Veuillez vérifier votre boîte mail.");

        if (user.LockoutEnabled)
            throw new UnauthorizedException("Le compte est désactivé. Contactez l’administrateur.");

        var roles = await _userManager.GetRolesAsync(user);
        

        var jwtToken = new JwtTokenModel
        {
            Email = user.Email,
            UserId = user.Id,
            Roles = roles.ToList(),
        };

        var resultToken = await _authServices.GetTokenAsync(jwtToken);
        var refreshTokenHash = AuthHelper.HashToken(resultToken.RefreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = refreshTokenHash,
            Email = jwtToken.Email,
            UserId = jwtToken.UserId, // Ou un vrai UserId si vous en avez
            Role = string.Join(";", roles),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenContext.AddDataAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new SignInResult(resultToken.Token);
    }

    private async Task<string> GenerateTokenAsync(SignInDTO signIn)
    {
        if (signIn == null) return null;

        var user = await _userManager.FindByNameAsync(signIn.Email);
        if (user == null) return null;


        var authClains = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Upn, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            authClains.Add(new Claim(ClaimTypes.Role, role));
        }
        return GetToken(authClains);
    }

    private string GetToken(List<Claim> authClains)
    {
        var authSigninkey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secureSecretProvider.GetSecretAsync(_configuration["JWT:Secret"]!).Result));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddDays(90),
            claims: authClains,
            signingCredentials: new SigningCredentials(authSigninkey, SecurityAlgorithms.HmacSha256Signature)
        );

        var strToken = new JwtSecurityTokenHandler().WriteToken(token);

        return strToken;
    }

}
