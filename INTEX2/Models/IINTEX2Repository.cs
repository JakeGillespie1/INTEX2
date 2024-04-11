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
        IQueryable<ProductBasedRecommendation> ProductBasedRecommendations { get; }

        public IEnumerable<object> GetMostPurchasedProducts();

        public IEnumerable<object> GetTopRatedProducts();

        public List<Product> GetProductRecs(string rec1, string rec2, string rec3);

        public void AddOrder(Order order);

        public void AddFraudPredictionToOrder(int orderId);

        Product GetProductById(int id);
    }
}
