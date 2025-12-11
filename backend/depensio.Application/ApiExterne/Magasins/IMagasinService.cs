using IDR.Library.Shared.Responses;
using Refit;

namespace depensio.Application.ApiExterne.Magasins;

public interface IMagasinService
{
    [Get("/magasin/{boutiqueId}")]
    Task<BaseResponse<GetMagasinsResponse>> GetMagasinsByBoutiqueAsync(Guid boutiqueId);

    [Get("/magasin/{boutiqueId}/{id}")]
    Task<BaseResponse<GetMagasinByIdResponse>> GetMagasinByIdAsync(Guid boutiqueId, Guid id);

    [Post("/magasin/{boutiqueId}")]
    Task<BaseResponse<CreateMagasinResponse>> CreateMagasinAsync(Guid BoutiqueId, [Body] CreateStockLocationRequest request);

    [Patch("/magasin/{boutiqueId}/{stockLocationId}")]
    Task<BaseResponse<UpdateMagasinResponse>> UpdateMagasinAsync(Guid boutiqueId, Guid stockLocationId, [Body] UpdateStockLocationRequest request);
}

public record CreateStockLocationRequest(StockLocationCreateDTO StockLocation);
public record UpdateStockLocationRequest(StockLocationUpdateDTO StockLocation);
public record GetMagasinsResponse(List<StockLocationDTO> StockLocations);
public record GetMagasinByIdResponse(StockLocationDTO StockLocation);
public record CreateMagasinResponse(Guid Id);
public record UpdateMagasinResponse(Guid Id);

public record StockLocationDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int Type { get; init; } = 1;
}

public record StockLocationUpdateDTO
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public record StockLocationCreateDTO
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int Type { get; init; } = 1;
}
