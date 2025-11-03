

namespace depensio.Application.UseCases.Auth.Commands.Refresh;
public record RefreshTokenCommand()
    : ICommand<RefreshTokenResult>;

public record RefreshTokenResult(string AccessToken);

