using depensio.Application.UseCases.Auth.DTOs;

namespace depensio.Application.UseCases.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(ResetPasswordDTO ResetPassword)
    : ICommand<ResetPasswordResult>;

public record ResetPasswordResult(bool Result);

