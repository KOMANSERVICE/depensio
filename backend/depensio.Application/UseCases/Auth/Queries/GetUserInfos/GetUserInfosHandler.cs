using depensio.Application.UseCases.Auth.DTOs;

namespace depensio.Application.UserCases.Auth.Queries.GetUserInfos;

public class GetUserInfosHandler(
        IUserContextService _userContextService,
        UserManager<ApplicationUser> _userManager,
        IEncryptionService _encryptionService
    )
    : IQueryHandler<GetUserInfosQuery, GetUserInfosResult>
{
    public async Task<GetUserInfosResult> Handle(GetUserInfosQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if(user is null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }



        return new GetUserInfosResult( new UserInfosDTO
        {
            FirstName = _encryptionService.Decrypt(user.FirstName),
            LastName = _encryptionService.Decrypt(user.LastName),
            Tel = _encryptionService.Decrypt(user.PhoneNumber ?? ""),
            Email = user.Email ?? "",
        }
        );
    }
}
