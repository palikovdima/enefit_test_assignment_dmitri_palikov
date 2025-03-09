import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { CartProvider } from "../context/CartContext";
import CartPage from "../cart/CartPage";
import CheckoutPage from "../checkout/CheckoutPage";
import ProductsPage from "./ProductsPage";
import { PRODUCTS_PAGE_URL, CART_PAGE_URL, CHECKOUT_PAGE_URL } from "../config/config";

function ProductsPageRouter() {
    return (
        <CartProvider>
            <Router>
                <Routes>
                    <Route path="/" element={<ProductsPage />} />
                    <Route path={PRODUCTS_PAGE_URL} element={<ProductsPage />} />
                    <Route path={CART_PAGE_URL} element={<CartPage />} />
                    <Route path={CHECKOUT_PAGE_URL} element={<CheckoutPage />} />
                </Routes>
            </Router>
        </CartProvider>
    );
}

export default ProductsPageRouter;
