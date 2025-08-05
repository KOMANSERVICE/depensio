
using System.ComponentModel.DataAnnotations;

namespace depensio.Shared.Enums;

public enum BarcodeGenerationMode
{
    [Display(Name = "Manuel")]
    Manual,     // Saisie manuelle
    [Display(Name = "Automatique")]
    Auto,       // Génération automatique
    [Display(Name = "Hybride")]
    Hybrid      // Auto avec possibilité de modification
}

public class ProductFormModel
{
    public BarcodeGenerationMode GenerationMode { get; set; } = BarcodeGenerationMode.Manual;

    public bool StockAuto { get; set; } = true;
}
