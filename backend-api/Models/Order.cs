namespace ProductCatalogAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OrderItem
    {
        public int Id { get; set; }  // ADD THIS LINE
        public int ProductId { get; set; }
        public string? ProductTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }  // ADD THIS LINE (foreign key)
    }

    public class CreateOrderRequest
    {
        public List<OrderItem> Items { get; set; } = new();
    }
}