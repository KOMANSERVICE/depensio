namespace depensio.Domain.Models;

public class Transaction : Entity<Guid>
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public DateTime Date { get; set; }
    public string Note { get; set; }

    public Account Account { get; set; }
}
