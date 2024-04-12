using INTEX2.Models.ViewModels;

namespace INTEX2.Models
{
    public interface IINTEX2Repository
    {
        IQueryable<Category> Categories { get; }

        List<Product> Products { get; }

        List<Customer> Customers { get; }

        List<Order> Orders { get; }

        IQueryable<LineItem> LineItems { get; }
        IQueryable<ProductBasedRecommendation> ProductBasedRecommendations { get; }

        public IEnumerable<object> GetMostPurchasedProducts();

        public IEnumerable<object> GetTopRatedProducts();

        public List<Product> GetProductRecs(string rec1, string rec2, string rec3);

        public void AddOrder(Order order);

        public void AddFraudPredictionToOrder(int orderId);

        Product GetProductById(int id);

        public void EditCustomer(Customer customer);

        public void DeleteCustomer(Customer customer);

        public void EditProduct(Product product);

        public void DeleteProduct(Product product);

        public AuthUserRecommendation GetMostRecent();

        public List<Order> FraudOrders();
    }
}
