

namespace depensio.Application.UseCases.Auth.Commands.Refresh;
public record RefreshTokenCommand(bool RemenberMe)
    : ICommand<RefreshTokenResult>;

public record RefreshTokenResult(string AccessToken);

