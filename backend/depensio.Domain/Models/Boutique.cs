using depensio.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace depensio.Domain.Models;

[Table("TB00001")]
public class Boutique : Entity<BoutiqueId>
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string OwnerId { get; set; }
    public string PublicLink { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = false;

    public ICollection<UsersBoutique> UsersBoutiques { get; set; }
    //public ICollection<LoyaltyReward> LoyaltyRewards { get; set; }
    public ICollection<Subscription> Subscriptions { get; set; }
    public virtual ICollection<Profile> Profiles { get; set; } = new HashSet<Profile>();
    public ICollection<Product> Products { get; set; }
    public ICollection<Sale> Sales { get; set; }
    public ICollection<BoutiqueSetting> BoutiqueSettings { get; set; }
    public ICollection<Purchase> Purchases { get; set; }
    //public ICollection<Account> Accounts { get; set; }
    //public ICollection<CashFlow> CashFlows { get; set; }
    //public ICollection<Quote> Quotes { get; set; }
}