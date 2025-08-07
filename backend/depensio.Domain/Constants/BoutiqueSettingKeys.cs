namespace depensio.Domain.Constants;

public static class BoutiqueSettingKeys
{
    // Gestion des produits
    public const string PRODUCT_KEY = "product.key";

    public const string PRODUCT_BARCODE_GENERATION_MODE = "product.barcode.generation.mode";
    public const string PRODUCT_STOCK_AUTOMATIQUE = "product.stock.automatique";

    // Gestion des ventes
    public const string VENTE_KEY = "vente.key";

    public const string VENTE_AUTORISER_MODIFICATION_PRIX = "vente.autoriser.modification.prix";
    public const string VENTE_AUTORISER_VENTE_AVEC_STOCK_ZERO = "vente.autoriser.vente.avec.stock.zero";

    //// Gestion des ventes (concept métier)
    //public const string SALE_TAX_RATE = "sale.tax.rate";
    //public const string SALE_DISCOUNT_ENABLED = "sale.discount.enabled";

    //// Gestion des stocks (concept métier)
    //public const string STOCK_ALERT_THRESHOLD = "stock.alert.threshold";
    //public const string STOCK_NEGATIVE_ALLOWED = "stock.negative.allowed";

    //// Interface utilisateur (concept métier)
    //public const string THEME_COLOR = "ui.theme.color";
    //public const string LANGUAGE = "ui.language";
    //public const string CURRENCY = "ui.currency";
}
