
using depensio.Application.UseCases.Auth.Services;
using IDR.Library.BuildingBlocks.Security.Interfaces;
using System.Buffers.Text;

namespace depensio.Application.UseCases.Auth.Commands.CreateUser;

public class CreateUserHandler(
    UserManager<ApplicationUser> _userManager,
    IEmailService _mailService,
    IConfiguration _configuration,
    IEncryptionService _encryptionService,
    IDepensioDbContext _dbContext,
    IGenericRepository<UsersBoutique> _usersBoutiqueRepo,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    IUserService _userService,
    ISecureSecretProvider _secureSecretProvider)
    : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var createUser = request.Signup;

        var userId = _userContextService.GetUserId();
        var boutique = _dbContext.Boutiques
                   .Any(b => b.Id == BoutiqueId.Of(createUser.BoutiqueId)
                               && b.UsersBoutiques.Any(ub => ub.UserId == userId));
        if (!boutique)
        {
            throw new UnauthorizedException("You are not authorized to add this company");
        }

        var userM = await _userManager.FindByEmailAsync(createUser.Email);

        if (userM is not null)
        {
            var userBoutique = AddNewBoutiqueToUser(createUser, userM.Id);
            await _usersBoutiqueRepo.AddDataAsync(userBoutique, cancellationToken);
            await _unitOfWork.SaveChangesDataAsync(cancellationToken);

            await SendMailAsync(userM, "AddBoutique.html");
            return new CreateUserResult(true);
        }

        var user = new ApplicationUser
        {
            UserName = createUser.Email,
            Email = createUser.Email
        };
        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            userM = await _userManager.FindByEmailAsync(createUser.Email);
            if (userM is null)
                throw new BadRequestException($"Impossible de creer l`utilisateur");

            var userBoutique = AddNewBoutiqueToUser(createUser, userM.Id);
            await _usersBoutiqueRepo.AddDataAsync(userBoutique, cancellationToken);
            await _unitOfWork.SaveChangesDataAsync(cancellationToken);

            await _userService.GenerateEmailConfirmationTokenAsync(userM);

            return new CreateUserResult(true);
        }

        throw new BadRequestException($"Impossible de creer l`utilisateur");
    }


    private UsersBoutique AddNewBoutiqueToUser(SignUpBoutiqueDTO signUpBoutiqueDTO, string userId)
    {
        return new UsersBoutique
        {
            Id = UsersBoutiqueId.Of(Guid.NewGuid()),
            UserId = userId,
            BoutiqueId = BoutiqueId.Of(signUpBoutiqueDTO.BoutiqueId)
        };
    }

    private async Task SendMailAsync(ApplicationUser user, string template)
    {
        var Frontend_BaseUrl = _configuration["Frontend:BaseUrl"]!;
        var BaseUrl = await _secureSecretProvider.GetSecretAsync(Frontend_BaseUrl);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var values = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "date", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") },
                { "link", $"{BaseUrl}/reset-password/{user.Id}?code={encodedToken}" }
            };
        var mailContent = await _mailService.RenderHtmlTemplateAsync(template, values);
        var mail = new EmailModel
        {
            ToMailIds = new List<string>()
                {
                    user.Email
                },
            Suject = mailContent.Subject,
            Body = mailContent.Body,
            IsBodyHtml = true
        };
        await _mailService.SendEmailAsync(mail);
    }
}
