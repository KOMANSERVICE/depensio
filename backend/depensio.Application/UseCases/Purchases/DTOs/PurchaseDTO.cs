using depensio.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace depensio.Application.UseCases.Purchases.DTOs;

public record PurchaseDTO
{
    public Guid Id { get; set; }
    public Guid BoutiqueId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    public IEnumerable<PurchaseItemDTO> Items { get; set; } = new List<PurchaseItemDTO>();
}
