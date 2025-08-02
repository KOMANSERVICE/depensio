
namespace depensio.Domain.Abstractions;

public abstract class BoutiqueSettingBase
{
    public virtual string Key { get; set; } = string.Empty; // Clé unique pour identifier le paramètre de la boutique, par exemple "product.key" ou "sale.tax.rate".
    public virtual string Value { get; set; } = string.Empty; // Valeur du paramètre, par exemple "true" ou "false" pour un booléen, ou une chaîne de caractères pour d'autres types.
}
