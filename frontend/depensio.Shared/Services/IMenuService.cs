namespace depensio.Shared.Services;

public interface IMenuService
{
    [Get("/menu/{boutiqueId}")]
    public Task<BaseResponse<GetMenuByBoutiqueResponse>> GetMenuByBoutiqueAsync(Guid boutiqueId);

    [Get("/menu/{boutiqueId}/user")]
    public Task<BaseResponse<GetMenuByUserBoutiqueResponse>> GetMenuByUserBoutiqueAsync(Guid boutiqueId);

    [Get("/menu/{boutiqueId}/user/{currentPath}")]
    public Task<BaseResponse<GetOneMenuByUserBoutiqueResponse>> GetOneMenuByUserBoutiqueAsync(Guid boutiqueId, string currentPath);
}
