using System;
using System.Collections.Generic;

namespace INTEX2.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string Date { get; set; } = null!;

    public string DayOfWeek { get; set; } = null!;

    public byte Time { get; set; }

    public string EntryMode { get; set; } = null!;

    public int? Amount { get; set; }

    public string TypeOfTransaction { get; set; } = null!;

    public string CountryOfTransaction { get; set; } = null!;

    public string? ShippingAddress { get; set; }

    public string Bank { get; set; } = null!;

    public string TypeOfCard { get; set; } = null!;

    public short Fraud { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();
}
