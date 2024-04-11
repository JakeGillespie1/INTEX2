using INTEX2.Models.ViewModels;

namespace INTEX2.Models
{
    public interface IINTEX2Repository
    {
        IQueryable<Category> Categories { get; }

        IEnumerable<Product> Products { get; }

        IQueryable<Customer> Customers { get; }

        IQueryable<Order> Orders { get; }

        IQueryable<LineItem> LineItems { get; }

        public IEnumerable<object> GetMostPurchasedProducts();

        public IEnumerable<object> GetTopRatedProducts();

        public void AddOrder(Order order);

        public void AddFraudPredictionToOrder(int orderId);

        Product GetProductById(int id);
    }
}
