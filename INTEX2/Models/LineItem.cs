using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INTEX2.Models;

public partial class LineItem
{
    [Key]
    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }

    [Key]
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Qty { get; set; }

    public byte Rating { get; set; }
}
