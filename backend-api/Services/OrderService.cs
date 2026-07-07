using ProductCatalogAPI.Data;
using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Services
{
    public interface IOrderService
    {
        Order CreateOrder(CreateOrderRequest request);
        Order? GetOrderById(int id);
        List<Order> GetAllOrders();
    }

    public class OrderService : IOrderService
    {
        private readonly AppDbContext _dbContext;

        public OrderService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Order CreateOrder(CreateOrderRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new ArgumentException("Order must contain at least one item");

            var order = new Order
            {
                Items = request.Items,
                Total = request.Items.Sum(item => item.Price * item.Quantity)
            };

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();
            return order;
        }

        public Order? GetOrderById(int id)
        {
            return _dbContext.Orders
                .Where(o => o.Id == id)
                .FirstOrDefault();
        }

        public List<Order> GetAllOrders()
        {
            return _dbContext.Orders.ToList();
        }
    }
}