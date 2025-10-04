namespace depensio.Shared.Services;

public interface IMenuService
{
    [Get("/menu/{boutiqueId}")]
    public Task<BaseResponse<GetMenuByBoutiqueResponse>> GetMenuByBoutiqueAsync(Guid boutiqueId);
}
