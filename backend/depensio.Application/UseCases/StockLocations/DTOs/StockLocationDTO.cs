namespace depensio.Application.UseCases.StockLocations.DTOs;

public record StockLocationDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}
