using System;
using System.Collections.Generic;

namespace INTEX2.Models;

public partial class ProductBasedRecommendation
{
    // Set ProductId as the primary key
    public int ProductId { get; set; }

    public string? Rec1 { get; set; }

    public string? Rec2 { get; set; }

    public string? Rec3 { get; set; }
}

