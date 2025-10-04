namespace depensio.Shared.Services;

public interface IProfileService
{
    [Get("/profile/{boutiqueId}")]
    public Task<BaseResponse<GetProfileByBoutiqueResponse>> GetProfileByBoutiqueAsync(Guid boutiqueId);
    [Post("/profile/{boutiqueId}")]
    public Task<BaseResponse<CreateProfileResponse>> CreateProfileAsync(Guid boutiqueId, CreateProfileRequest request);
}
