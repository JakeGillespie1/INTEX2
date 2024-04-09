using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace INTEX2.Models;

public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public short Year { get; set; }

    public short NumParts { get; set; }

    public short Price { get; set; }

    public string ImgLink { get; set; } = null!;

    public string PrimaryColor { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
