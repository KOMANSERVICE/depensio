using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace depensio.Application.Auth.Queries.SignIn;

public class SignInHandler(
        SignInManager<ApplicationUser> _signInManager,
        IConfiguration _configuration,
        UserManager<ApplicationUser> _userManager
    )    
    : IQueryHandler<SignInQuery, SignInResult>
{
    public async Task<SignInResult> Handle(SignInQuery query, CancellationToken cancellationToken)
    {
        var signIn = query.SignIn;

        var result = await _signInManager.PasswordSignInAsync(signIn.Email, signIn.Password, true, false);
        if (!result.Succeeded)
        {
            throw new BadRequestException($"SignIn : Connection fail");
        }

        return new SignInResult(await GenerateTokenAsync(signIn));
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
        var authSigninkey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]!));

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
