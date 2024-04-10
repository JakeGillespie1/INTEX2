using System;
using System.Collections.Generic;

namespace INTEX2.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string BirthDate { get; set; } = null!;

    public string CountryOfResidence { get; set; } = null!;

    public string? Gender { get; set; }

    public byte Age { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
