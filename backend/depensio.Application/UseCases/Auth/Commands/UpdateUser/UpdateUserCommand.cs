using depensio.Application.UseCases.Auth.DTOs;
using depensio.Application.UseCases.Auth.Services;

namespace depensio.Application.UseCases.Auth.Commands.UpdateUser;

public record UpdateUserCommand(UserInfosDTO UserInfos)
    : ICommand<UpdateUserResult>;

public record UpdateUserResult(bool Result);

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserInfos.FirstName)
            .NotEmpty().WithMessage("Le prénom est obligatoire.");
        
        RuleFor(x => x.UserInfos.LastName)
            .NotEmpty().WithMessage("Le nom est obligatoire.");
        
        RuleFor(x => x.UserInfos.Tel)
            .NotEmpty().WithMessage("Le numéro de téléphone est obligatoire.");
    }
}