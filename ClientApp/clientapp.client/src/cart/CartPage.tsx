import { useNavigate } from 'react-router-dom';
import { useEffect} from 'react';
import './CartPage.css';
import { useCart } from '../context/CartContext';
import { CHECKOUT_PAGE_URL, PRODUCTS_PAGE_URL } from '../config/config';

function CartPage() {
    const { cart, clearCart, fetchCart, totalAmount } = useCart();
    const navigate = useNavigate();

    useEffect(() => {
        fetchCart();
    }, []);

    return (
        <div className="cart-container">
            <header>
                <h1>Shopping Cart</h1>
            </header>
            <main className="content">
                {cart.length === 0 ? (
                    <p>Your cart is empty.</p>
                ) : (
                    <div className="cart-grid">
                        {cart.map(product => (
                            <div className="cart-item" key={product.id}>
                                {product.imageSource && (
                                    <img
                                        src={product.imageSource}
                                        alt={product.name}
                                        className="product-image"
                                        loading="lazy"
                                        onError={(e) => e.currentTarget.style.display = "none"}
                                    />
                                )}
                                <h2>{product.name}</h2>
                                <p>Price: {"\u20AC"}{product.price}</p>
                                <p>Quantity: {product.quantity}</p>
                            </div>
                        ))}
                    </div>
                )}
            </main>
            <footer className="footer">
                <p className="total-amount">Total Amount: {"\u20AC"}{totalAmount}</p>
                <button onClick={() => navigate({ pathname: PRODUCTS_PAGE_URL })}>Back to Store</button>
                <button className="checkout-button" onClick={() => navigate({ pathname: CHECKOUT_PAGE_URL })}>Checkout</button>
                <button className="reset-button" onClick={clearCart}>Clear Cart</button>
            </footer>
        </div>
    );
}

export default CartPage;
