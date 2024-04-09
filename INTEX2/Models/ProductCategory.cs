using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INTEX2.Models
{
    public class ProductCategory
    {
        [Key]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Key]
        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
