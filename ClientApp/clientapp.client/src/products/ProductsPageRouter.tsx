import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { CartProvider } from "../context/CartContext";
import CartPage from "../cart/CartPage";
import CheckoutPage from "../checkout/CheckoutPage";
import ProductsPage from "./ProductsPage";

function ProductsPageRouter() {
    return (
        <CartProvider>
            <Router>
                <Routes>
                    <Route path="/" element={<ProductsPage />} />
                    <Route path="/products" element={<ProductsPage />} />
                    <Route path="/cart" element={<CartPage />} />
                    <Route path="/checkout" element={<CheckoutPage />} />
                </Routes>
            </Router>
        </CartProvider>
    );
}

export default ProductsPageRouter;
