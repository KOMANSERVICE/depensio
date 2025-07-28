namespace depensio.Domain.Constants;

public static class BoutiqueSettingKeys
{
    // Gestion des codes-barres (concept métier)
    public const string BARCODE_GENERATION_MODE = "barcode.generation.mode";
    public const string BARCODE_PREFIX = "barcode.prefix";

    // Gestion des ventes (concept métier)
    public const string SALE_TAX_RATE = "sale.tax.rate";
    public const string SALE_DISCOUNT_ENABLED = "sale.discount.enabled";

    // Gestion des stocks (concept métier)
    public const string STOCK_ALERT_THRESHOLD = "stock.alert.threshold";
    public const string STOCK_NEGATIVE_ALLOWED = "stock.negative.allowed";

    // Interface utilisateur (concept métier)
    public const string THEME_COLOR = "ui.theme.color";
    public const string LANGUAGE = "ui.language";
    public const string CURRENCY = "ui.currency";
}
