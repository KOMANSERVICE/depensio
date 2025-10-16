using System.ComponentModel;

namespace depensio.Domain.Enums;

public enum StockMovementType
{
    [Description("Entrée")]
    Entry = 1,
    [Description("Sortie")]
    Exit = 2,
    [Description("Transfert")]
    Transfer = 3,
    [Description("Inventaire")]
    Inventory = 4 //a verifier si on en a besoin
}
