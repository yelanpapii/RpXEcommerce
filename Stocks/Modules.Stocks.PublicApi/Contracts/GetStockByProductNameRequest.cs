using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Stocks.PublicApi.Contracts;

public sealed record GetStocksByProductIdRequest(Guid ProductId, Guid? VariantId);
