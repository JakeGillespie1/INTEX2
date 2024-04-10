using INTEX2.Models.ViewModels;

namespace INTEX2.Models
{
    public interface IINTEX2Repository
    {
        IQueryable<Category> Categories { get; }

        IQueryable<Product> Products { get; }

        IQueryable<Customer> Customers { get; }

        IQueryable<Order> Orders { get; }

        IQueryable<LineItem> LineItems { get; }

        public IEnumerable<object> GetMostPurchasedProducts();

        public IEnumerable<object> GetTopRatedProducts();
    }
}
