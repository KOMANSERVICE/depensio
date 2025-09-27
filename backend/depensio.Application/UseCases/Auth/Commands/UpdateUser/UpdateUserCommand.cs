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
        RuleFor(x => x.UserInfos.FirstName).NotEmpty().WithMessage("FirstName is required");
        RuleFor(x => x.UserInfos.LastName).NotEmpty().WithMessage("LastName is required");
        RuleFor(x => x.UserInfos.Tel).NotEmpty().WithMessage("Tel is required");
    }

}