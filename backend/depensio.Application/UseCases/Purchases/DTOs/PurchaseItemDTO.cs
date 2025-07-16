using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace depensio.Application.UseCases.Purchases.DTOs;

public record PurchaseItemDTO(
    Guid Id,
    Guid ProductId,
    decimal Price,
    int Quantity
);
