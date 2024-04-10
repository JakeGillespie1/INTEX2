using System;
using System.Collections.Generic;

namespace INTEX2.Models;

public partial class LineItem
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public byte Qty { get; set; }

    public byte Rating { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
