using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace depensio.Application.UseCases.Sales.DTOs;

public record SaleDetailDTO(Guid Id, string Name, int Quantity,decimal price, decimal Revenue, DateTime Date);
