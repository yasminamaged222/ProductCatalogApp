import { Link } from "react-router-dom";
import type { Product } from "./data";

interface CartPageProps {
  cart: Product[];
}

function CartPage({ cart }: CartPageProps) {
  const total = cart.reduce((sum, p) => sum + p.price, 0);

  return (
    <div className="app">
      <h1>Your Cart</h1>
      <Link to="/">← Back to Catalog</Link>

      {cart.length === 0 && <p>Your cart is empty.</p>}

      {cart.map((item, index) => (
        <div key={index} className="product-card">
          <h3>{item.title}</h3>
          <p>${item.price}</p>
        </div>
      ))}

      {cart.length > 0 && <h2>Total: ${total.toFixed(2)}</h2>}
    </div>
  );
}

export default CartPage;