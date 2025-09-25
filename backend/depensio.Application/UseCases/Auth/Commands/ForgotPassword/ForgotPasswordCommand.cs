using depensio.Application.UseCases.Auth.DTOs;

namespace depensio.Application.UseCases.Auth.Commands.ForgetPassword;


public record ForgotPasswordCommand(ForgotPasswordDTO ForgotPassword)
    : ICommand<ForgotPasswordResult>;

public record ForgotPasswordResult(bool Result);

