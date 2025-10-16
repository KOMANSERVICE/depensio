using System.ComponentModel;

namespace depensio.Domain.Enums;

public enum StockLocationType
{
  
    [Description("Entrepôt / Magasin de stockage")]
    Store = 1,// cette fonctionnalité est réservée aux abonnés Pro, Entreprise et est payante
    [Description("Chantier")]
    Site = 2,
    //[Description("Entrepôt")]
    //Warehouse = 1,
    //[Description("Fournisseur")]
    //Supplier,
    //[Description("Client")]
    //Customer
}
