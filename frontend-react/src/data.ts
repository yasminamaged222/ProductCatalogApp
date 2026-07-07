export interface Product {
  id: number;
  title: string;
  price: number;
  category: string;
}

export const products: Product[] = [
  { id: 1, title: "Laptop", price: 1000, category: "electronics" },
  { id: 2, title: "T-Shirt", price: 20, category: "clothing" },
  { id: 3, title: "Headphones", price: 150, category: "electronics" },
  { id: 4, title: "Jeans", price: 60, category: "clothing" },
];