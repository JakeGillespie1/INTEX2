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

        public IQueryable<Product> Products => _context.Products;

        public IQueryable<Customer> Customers => _context.Customers;

        public IQueryable<Order> Orders => _context.Orders;

        public IQueryable<LineItem> LineItems => _context.LineItems;

    }
}

