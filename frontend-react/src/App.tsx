import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import ProductCard from "./ProductCard";
import type { Product } from "./data";
import "./App.css";

function App() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [cart, setCart] = useState<Product[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [category, setCategory] = useState("all");
  const [checkoutLoading, setCheckoutLoading] = useState(false);
  const [checkoutMessage, setCheckoutMessage] = useState("");

  useEffect(() => {
    const loadProducts = async () => {
      try {
        const response = await fetch("http://localhost:5135/api/products");
        const data = await response.json();
        setProducts(data.products);
      } catch (error) {
        console.error("Failed to load products:", error);
      } finally {
        setLoading(false);
      }
    };
    loadProducts();
  }, []);

  const handleAddToCart = (product: Product) => {
    setCart((prevCart) => [...prevCart, product]);
  };

  const handleCheckout = async () => {
    if (cart.length === 0) {
      setCheckoutMessage("Cart is empty!");
      return;
    }

    setCheckoutLoading(true);
    setCheckoutMessage("");

    try {
      const orderItems = cart.map((product) => ({
        productId: product.id,
        productTitle: product.title,
        price: product.price,
        quantity: 1,
      }));

      const response = await fetch("http://localhost:5135/api/products/orders", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ items: orderItems }),
      });

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.error || "Checkout failed");
      }

      const order = await response.json();
      setCheckoutMessage(`✅ Order #${order.id} created! Total: $${order.total}`);
      setCart([]); // Clear cart after successful checkout
    } catch (error) {
      setCheckoutMessage(`❌ Checkout failed: ${error instanceof Error ? error.message : "Unknown error"}`);
      console.error("Checkout error:", error);
    } finally {
      setCheckoutLoading(false);
    }
  };

  const total = cart.reduce((sum, p) => sum + p.price, 0);
  const categories = [...new Set(products.map((p) => p.category))];

  const filteredProducts = products
    .filter((p) => p.title.toLowerCase().includes(searchTerm.toLowerCase()))
    .filter((p) => category === "all" || p.category === category);

  if (loading) {
    return <p>Loading...</p>;
  }

  return (
    <div className="app">
      <div className="header">
        <h1>Product Catalog</h1>
        <div>
          <p>🛒 Cart: {cart.length} items — ${total.toFixed(2)}</p>
          <button 
            onClick={handleCheckout} 
            disabled={checkoutLoading || cart.length === 0}
            style={{
              padding: "8px 16px",
              backgroundColor: cart.length === 0 ? "#ccc" : "#28a745",
              color: "white",
              border: "none",
              borderRadius: "4px",
              cursor: checkoutLoading || cart.length === 0 ? "not-allowed" : "pointer",
            }}
          >
            {checkoutLoading ? "Processing..." : "Checkout"}
          </button>
        </div>
      </div>

      {checkoutMessage && (
        <div
          style={{
            padding: "12px",
            marginBottom: "16px",
            backgroundColor: checkoutMessage.includes("✅") ? "#d4edda" : "#f8d7da",
            color: checkoutMessage.includes("✅") ? "#155724" : "#721c24",
            borderRadius: "4px",
          }}
        >
          {checkoutMessage}
        </div>
      )}

      <div className="controls">
        <input
          type="text"
          placeholder="Search products..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
        <select value={category} onChange={(e) => setCategory(e.target.value)}>
          <option value="all">All Categories</option>
          {categories.map((cat) => (
            <option key={cat} value={cat}>
              {cat}
            </option>
          ))}
        </select>
      </div>

      <div className="product-grid">
        {filteredProducts.map((product) => (
          <ProductCard key={product.id} product={product} onAddToCart={handleAddToCart} />
        ))}
      </div>

      {filteredProducts.length === 0 && <p>No products found.</p>}
    </div>
  );
}

export default App;