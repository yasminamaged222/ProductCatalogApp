import { useState } from "react";
import type { Product } from "./data";

interface ProductCardProps {
  product: Product;
  onAddToCart: (product: Product) => void;
}

function ProductCard({ product, onAddToCart }: ProductCardProps) {
  const [added, setAdded] = useState(false);

  const handleClick = () => {
    setAdded(true);
    onAddToCart(product);
  };

 return (
  <div className="product-card">
    <h3>{product.title}</h3>
    <p>${product.price}</p>
    <span>{product.category}</span>
    <br />
    <button onClick={handleClick} disabled={added}>
      {added ? "Added ✓" : "Add to Cart"}
    </button>
  </div>
);
}

export default ProductCard;