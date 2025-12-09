using System.ComponentModel.DataAnnotations;

namespace depensio.Shared.Pages.Magasin.Models;

public record CreateMagasinResponse(Guid Id);
public record CreateMagasinRequest(StockLocationCreateDTO StockLocation);
public record UpdateMagasinRequest(StockLocationUpdateDTO StockLocation, Guid Id);

public record GetMagasinsByBoutiqueResponse(IEnumerable<StockLocationDTO> StockLocations);

public record StockLocationDTO
{
    public Guid Id { get; set; } = Guid.Empty;

    [Required(ErrorMessage = "Le nom est requis")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est requise")]
    public string Address { get; set; } = string.Empty;

    public int Type { get; set; } = 1;
}

public record StockLocationUpdateDTO
{
    [Required(ErrorMessage = "Le nom est requis")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est requise")]
    public string Address { get; set; } = string.Empty;
}

public record StockLocationCreateDTO
{
    public Guid Id { get; set; } = Guid.Empty;

    [Required(ErrorMessage = "Le nom est requis")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est requise")]
    public string Address { get; set; } = string.Empty;

    public int Type { get; set; } = 1;
}
