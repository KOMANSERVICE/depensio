using depensio.Domain.Enums;

namespace depensio.Domain.Settings;

public class BoutiqueBarcodeConfig
{
    public BarcodeGenerationMode GenerationMode { get; set; } = BarcodeGenerationMode.Auto;
    public string? Prefix { get; set; }
    public bool AutoIncrement { get; set; } = true;
    public int NextBarcodeNumber { get; set; } = 1;
}
