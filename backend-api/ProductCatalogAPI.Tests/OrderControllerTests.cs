using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ProductCatalogAPI.Controllers;
using ProductCatalogAPI.Models;
using ProductCatalogAPI.Services;
using ProductCatalogAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalogAPI.Tests
{
    public class OrderControllerTests
    {
        private AppDbContext GetTestDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        // Test 1: CreateOrder returns HTTP 201 Created
        [Fact]
        public void CreateOrder_WithValidRequest_Returns201Created()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                var controller = new ProductsController(service);

                var request = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, ProductTitle = "Laptop", Price = 1000, Quantity = 1 }
                    }
                };

                // Act
                var result = controller.CreateOrder(request);

                // Assert
                var createdResult = Assert.IsType<CreatedAtActionResult>(result);
                Assert.Equal(nameof(controller.GetOrderById), createdResult.ActionName);
                Assert.Equal(201, createdResult.StatusCode);
            }
        }

        // Test 2: CreateOrder with empty items returns 400 BadRequest
        [Fact]
        public void CreateOrder_WithEmptyItems_Returns400BadRequest()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                var controller = new ProductsController(service);

                var request = new CreateOrderRequest
                {
                    Items = new List<OrderItem>()
                };

                // Act
                var result = controller.CreateOrder(request);

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        // Test 3: GetOrderById returns 200 OK with order data
        [Fact]
        public void GetOrderById_WithExistingId_Returns200Ok()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                var controller = new ProductsController(service);

                // Create an order
                var createRequest = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, ProductTitle = "Laptop", Price = 1000, Quantity = 1 }
                    }
                };
                var createdOrder = service.CreateOrder(createRequest);

                // Act
                var result = controller.GetOrderById(createdOrder.Id);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, okResult.StatusCode);
                
                var returnedOrder = Assert.IsType<Order>(okResult.Value);
                Assert.Equal(createdOrder.Id, returnedOrder.Id);
            }
        }

        // Test 4: GetOrderById with non-existing ID returns 404 NotFound
        [Fact]
        public void GetOrderById_WithNonExistingId_Returns404NotFound()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                var controller = new ProductsController(service);

                // Act
                var result = controller.GetOrderById(999);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }
        }

        // Test 5: GetAllOrders returns 200 OK with list
        [Fact]
        public void GetAllOrders_Returns200OkWithOrders()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                var controller = new ProductsController(service);

                // Create orders
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
                var result = controller.GetAllOrders();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var orders = Assert.IsType<List<Order>>(okResult.Value);
                Assert.Equal(2, orders.Count);
            }
        }

        // Test 6: CreateOrder saves to database and returns order with ID
        [Fact]
        public void CreateOrder_SavesOrderAndReturnsWithId()
        {
            // Arrange
            using (var context = GetTestDbContext())
            {
                var service = new OrderService(context);
                var controller = new ProductsController(service);

                var request = new CreateOrderRequest
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, ProductTitle = "Laptop", Price = 1000, Quantity = 1 }
                    }
                };

                // Act
                var result = controller.CreateOrder(request);
                var createdResult = Assert.IsType<CreatedAtActionResult>(result);
                var createdOrder = Assert.IsType<Order>(createdResult.Value);

                // Assert - Order has ID assigned
                Assert.True(createdOrder.Id > 0);

                // Verify it's in database
                var retrieved = service.GetOrderById(createdOrder.Id);
                Assert.NotNull(retrieved);
                Assert.Equal(1000, retrieved.Total);
            }
        }
    }
}