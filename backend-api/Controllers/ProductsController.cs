using Microsoft.AspNetCore.Mvc;
using ProductCatalogAPI.Models;
using ProductCatalogAPI.Services;

namespace ProductCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> Products = new()
        {
            new Product { Id = 1, Title = "Laptop", Price = 1000, Category = "electronics" },
            new Product { Id = 2, Title = "T-Shirt", Price = 20, Category = "clothing" },
            new Product { Id = 3, Title = "Headphones", Price = 150, Category = "electronics" },
            new Product { Id = 4, Title = "Jeans", Price = 60, Category = "clothing" },
            new Product { Id = 5, Title = "Watch", Price = 200, Category = "electronics" },
            new Product { Id = 6, Title = "Sweater", Price = 45, Category = "clothing" },
        };

        private readonly IOrderService _orderService;

        public ProductsController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(new { products = Products });
        }

        [HttpPost("orders")]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                if (request?.Items == null || request.Items.Count == 0)
                {
                    return BadRequest(new { error = "Order must contain at least one item" });
                }

                // Validate each item has required fields
                foreach (var item in request.Items)
                {
                    if (item.ProductId <= 0 || string.IsNullOrEmpty(item.ProductTitle) || item.Price <= 0 || item.Quantity <= 0)
                    {
                        return BadRequest(new { error = "Each item must have valid productId, productTitle, price, and quantity" });
                    }
                }

                var order = _orderService.CreateOrder(request);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpGet("orders/{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound(new { error = $"Order with id {id} not found" });
            }
            return Ok(order);
        }

        [HttpGet("orders")]
        public IActionResult GetAllOrders()
        {
            return Ok(_orderService.GetAllOrders());
        }
    }
}