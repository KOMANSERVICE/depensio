using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depensio.Domain.Models;

public class BoutiqueSetting : Entity<BoutiqueSettingId>
{

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
    public BoutiqueId BoutiqueId { get; set; }

    public virtual Boutique Boutique { get; set; }

}
