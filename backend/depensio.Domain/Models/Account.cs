namespace depensio.Domain.Models;

public class Account : Entity<Guid>
{
    public Guid BoutiqueId { get; set; }
    public string Label { get; set; }
    public string Type { get; set; }
    public decimal Balance { get; set; }

    public Boutique Boutique { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}
