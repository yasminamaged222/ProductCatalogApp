using Xunit;
using Moq;
using ProductCatalogAPI.Data;
using ProductCatalogAPI.Models;
using ProductCatalogAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalogAPI.Tests
{
    public class OrderServiceTests
    {
        // Helper: Create an in-memory DbContext for testing
        private AppDbContext GetTestDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        // Test 1: Create order with valid items
        [Fact]
        public void CreateOrder_WithValidItems_ReturnsOrderWithCorrectTotal()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                
                var request = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, ProductTitle = "Laptop", Price = 1000, Quantity = 1 },
                        new OrderItem { ProductId = 2, ProductTitle = "Mouse", Price = 50, Quantity = 2 }
                    }
                };

                // Act
                var result = service.CreateOrder(request);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1100, result.Total);  // 1000 + (50 * 2)
                Assert.Equal(2, result.Items.Count);
            }
        }

        // Test 2: Create order with empty items should throw exception
        [Fact]
        public void CreateOrder_WithEmptyItems_ThrowsArgumentException()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                
                var request = new CreateOrderRequest
                {
                    Items = new List<OrderItem>()  // Empty!
                };

                // Act & Assert
                var ex = Assert.Throws<ArgumentException>(() => service.CreateOrder(request));
                Assert.Equal("Order must contain at least one item", ex.Message);
            }
        }

        // Test 3: Create order with null items should throw exception
        [Fact]
        public void CreateOrder_WithNullItems_ThrowsArgumentException()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                
                var request = new CreateOrderRequest
                {
                    Items = null  // Null!
                };

                // Act & Assert
                Assert.Throws<ArgumentException>(() => service.CreateOrder(request));
            }
        }

        // Test 4: GetOrderById returns order when it exists
        [Fact]
        public void GetOrderById_WithExistingId_ReturnsOrder()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                
                // Create and save an order
                var request = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, ProductTitle = "Laptop", Price = 1000, Quantity = 1 }
                    }
                };
                var createdOrder = service.CreateOrder(request);

                // Act
                var result = service.GetOrderById(createdOrder.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(createdOrder.Id, result.Id);
                Assert.Equal(1000, result.Total);
            }
        }

        // Test 5: GetOrderById returns null when order doesn't exist
        [Fact]
        public void GetOrderById_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);

                // Act
                var result = service.GetOrderById(999);

                // Assert
                Assert.Null(result);
            }
        }

        // Test 6: GetAllOrders returns all orders
        [Fact]
        public void GetAllOrders_ReturnsAllCreatedOrders()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                
                // Create multiple orders
                var request1 = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, ProductTitle = "Laptop", Price = 1000, Quantity = 1 }
                    }
                };
                var request2 = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 2, ProductTitle = "Mouse", Price = 50, Quantity = 2 }
                    }
                };

                service.CreateOrder(request1);
                service.CreateOrder(request2);

                // Act
                var result = service.GetAllOrders();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
            }
        }

        // Test 7: Order total is calculated correctly with multiple items
        [Fact]
        public void CreateOrder_CalculatesTotalCorrectly()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                
                var request = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, ProductTitle = "Item1", Price = 100, Quantity = 3 },  // 300
                        new OrderItem { ProductId = 2, ProductTitle = "Item2", Price = 50, Quantity = 2 },   // 100
                        new OrderItem { ProductId = 3, ProductTitle = "Item3", Price = 25, Quantity = 4 }    // 100
                    }
                };

                // Act
                var result = service.CreateOrder(request);

                // Assert
                Assert.Equal(500, result.Total);  // 300 + 100 + 100
            }
        }

        // Test 8: Created order is persisted to database
        [Fact]
        public void CreateOrder_PersistsToDatabase()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            
            Order createdOrderId = null;

            // Create order in first context
            using (var context = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(dbName)
                    .Options))
            {
                var service = new OrderService(context);
                
                var request = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, ProductTitle = "Laptop", Price = 1000, Quantity = 1 }
                    }
                };

                createdOrderId = service.CreateOrder(request);
            }

            // Retrieve in second context (different instance, same DB)
            using (var context = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(dbName)
                    .Options))
            {
                var service = new OrderService(context);

                // Act
                var retrieved = service.GetOrderById(createdOrderId.Id);

                // Assert
                Assert.NotNull(retrieved);
                Assert.Equal(1000, retrieved.Total);
            }
        }
    }
}