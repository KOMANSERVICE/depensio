
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
    [Required]
    public BarcodeGenerationMode GenerationMode { get; set; } = BarcodeGenerationMode.Manual;

    // ... autres propriétés (ex: Barcode, Name, etc.)
}
