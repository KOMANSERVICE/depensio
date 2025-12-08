using IDR.Library.Shared.Responses;
using Refit;

namespace depensio.Application.ApiExterne.Magasins;

public interface IMagasinService
{
    [Get("/magasin/{boutiqueId}")]
    Task<BaseResponse<GetMagasinsResponse>> GetMagasinsByBoutiqueAsync(Guid boutiqueId);

    [Post("/magasin/{boutiqueId}")]
    Task<BaseResponse<CreateMagasinResponse>> CreateMagasinAsync(Guid boutiqueId, [Body] StockLocationCreateDTO stockLocation);
}

public record GetMagasinsResponse(List<StockLocationDTO> StockLocations);
public record CreateMagasinResponse(Guid Id);

public record StockLocationDTO : StockLocationUpdateDTO
{
    public Guid Id { get; init; }
    public StockLocationType Type { get; init; } = StockLocationType.Sale;
}

public record StockLocationUpdateDTO
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public record StockLocationCreateDTO
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public StockLocationType Type { get; init; } = StockLocationType.Sale;
}

public enum StockLocationType
{
    Sale = 1,
    Store = 2,
    Site = 3
}
