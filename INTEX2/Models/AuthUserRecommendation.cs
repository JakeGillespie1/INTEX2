using System;
using System.Collections.Generic;

namespace INTEX2.Models;

public partial class AuthUserRecommendation
{
    public long? Index { get; set; }

    public string? IfYouBought { get; set; }

    public string? Rec1 { get; set; }

    public string? Rec2 { get; set; }

    public string? Rec3 { get; set; }
}
