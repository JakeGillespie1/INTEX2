using INTEX2.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace INTEX2.Models
{
    public class EFINTEX2Repository : IINTEX2Repository
    {
        private readonly INTEX2Context _context;

        public EFINTEX2Repository(INTEX2Context temp)
        {
            _context = temp;
        }

        public IQueryable<Category> Categories => _context.Categories;

        public List<Product> Products => _context.Products.ToList();

        public List<Customer> Customers => _context.Customers.ToList();

        public IQueryable<Order> Orders => _context.Orders;

        public IQueryable<LineItem> LineItems => _context.LineItems;
        public IQueryable<ProductBasedRecommendation> ProductBasedRecommendations => _context.ProductBasedRecommendations;

        public IEnumerable<object> GetMostPurchasedProducts()
        {
            var topProducts = _context.LineItems
                .Join(_context.Products,
                    li => li.ProductId,
                    p => p.ProductId,
                    (li, p) => new
                    {
                        ProductID = p.ProductId,
                        Name = p.Name,
                        ImgLink = p.ImgLink,
                        Price = p.Price,
                        Rating = li.Rating,
                        Quantity = li.Qty,
                    })
                .GroupBy(result => new { result.ProductID, result.Name, result.ImgLink, result.Price })
                .Select(g => new
                {
                    TopProdId = g.Key.ProductID,
                    TopProdName = g.Key.Name,
                    TopProdImgTag = g.Key.ImgLink,
                    TopProdPrice = g.Key.Price,
                    TotalPurchased = g.Sum(item => item.Quantity)
                })
                .OrderByDescending(result => result.TotalPurchased)
                .Take(3)
                .ToList();

            return topProducts;
        }

        public IEnumerable<object> GetTopRatedProducts()
        {
            var topProductsHighestRating = _context.LineItems
                .Join(_context.Products,
                    li => li.ProductId,
                    p => p.ProductId,
                    (li, p) => new
                    {
                        ProductID = p.ProductId,
                        Name = p.Name,
                        ImgLink = p.ImgLink,
                        Price = p.Price,
                        Rating = li.Rating
                    })
                .GroupBy(result => new { result.ProductID, result.Name, result.ImgLink, result.Price })
                .Select(g => new
                {
                    TopProdId = g.Key.ProductID,
                    TopProdName = g.Key.Name,
                    TopProdImgTag = g.Key.ImgLink,
                    TopProdPrice = g.Key.Price,
                    RatingAvg = g.Average(item => item.Rating)
                })
                .OrderByDescending(result => result.RatingAvg)
                .Take(3)
                .ToList();

            return topProductsHighestRating;
        }
        public Product GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == id);
        }

        public List<Product> GetProductRecs(string rec1, string rec2, string rec3)
        {
            var recommendations = new List<Product>
            {
                _context.Products
                .FirstOrDefault(x => x.Name == rec1),

                _context.Products
                .FirstOrDefault(x => x.Name == rec2),

                _context.Products
                .FirstOrDefault(x => x.Name == rec3),
            };

            return (recommendations);
            
        }

        public void AddFraudPredictionToOrder(int orderId)
        {
            var rowToModify = _context.Orders.SingleOrDefault(x => x.OrderId == orderId);
            rowToModify.Fraud = 1;
            _context.SaveChanges();
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        public void EditCustomer(Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
        }

        public void DeleteCustomer(Customer customer) 
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }

        public void EditProduct(Product product)
        {
            _context.Update(product);
            _context.SaveChanges();
        }

    }
}

