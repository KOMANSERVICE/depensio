using depensio.Application.Interfaces;
using depensio.Application.UserCases.Auth.DTOs;
using IDR.Library.BuildingBlocks.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace depensio.Application.UserCases.Auth.Queries.SignIn;

public class SignInHandler(
        SignInManager<ApplicationUser> _signInManager,
        UserManager<ApplicationUser> _userManager,
        AuthorizationService _authServices,
        RefreshTokenService<RefreshToken> _refreshTokenService,
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

        var resultToken = await _authServices.GetTokenAsync(jwtToken, signIn.RememberMe);

        await _refreshTokenService.SaveRefreshTokenAsync(resultToken, jwtToken, cancellationToken);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new SignInResult(resultToken.Token);
    }
    

}
