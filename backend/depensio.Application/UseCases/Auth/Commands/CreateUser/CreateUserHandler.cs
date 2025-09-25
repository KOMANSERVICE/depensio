using depensio.Application.Interfaces;
using depensio.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace depensio.Application.UseCases.Auth.Commands.CreateUser;

public class CreateUserHandler(
    UserManager<ApplicationUser> _userManager,
    IEmailService _mailService,
    IConfiguration _configuration,
    IEncryptionService _encryptionService,
    ITemplateRendererService _templateRendererService,
    IDepensioDbContext _dbContext,
    IUserContextService _userContextService)
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
            AddNewBoutiqueToUser(createUser, Guid.Parse(userM.Id));
            await SendMailAsync(userM, "AddBoutique.html");

            return new CreateUserResult(true);
        }

        var user = new ApplicationUser
        {
            UserName = createUser.Email,
            Email = createUser.Email,
            LastName = _encryptionService.Encrypt(createUser.LastName),
            FirstName = _encryptionService.Encrypt(createUser.FirstName)
        };
        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            userM = await _userManager.FindByEmailAsync(createUser.Email);
            if (userM is null)
                throw new BadRequestException($"Impossible de creer l`utilisateur");

            AddNewBoutiqueToUser(createUser, Guid.Parse(userM.Id));
            await SendMailAsync(userM, "AccountCreated.html");

            return new CreateUserResult(true);
        }

        throw new BadRequestException($"Impossible de creer l`utilisateur");
    }


    private UsersBoutique AddNewBoutiqueToUser(SignUpBoutiqueDTO signUpBoutiqueDTO, Guid userId)
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
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var values = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "date", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") },
                { "link", $"{_configuration["JWT:ValidIssuer"]}/reset-password/{user.Id}?code={encodedToken}" }
            };
        var mailContent = await _templateRendererService.RenderTemplateAsync(template, values);
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
