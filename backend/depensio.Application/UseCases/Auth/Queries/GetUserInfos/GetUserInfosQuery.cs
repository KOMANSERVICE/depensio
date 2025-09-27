using depensio.Application.UseCases.Auth.DTOs;

namespace depensio.Application.UserCases.Auth.Queries.GetUserInfos;

public record GetUserInfosQuery()
    : IQuery<GetUserInfosResult>;

public record GetUserInfosResult(UserInfosDTO UserInfos);
