import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCart } from "../context/CartContext";
import "./CheckoutPage.css";
import { CHECKOUT_VALIDATE_PAYMENT_URL, CHECKOUT_CALCULATE_CHANGE_BACK_AMOUNT_URL, PRODUCTS_PAGE_URL } from '../config/config';

interface Bill {
    image: string;
    count: number;
}

interface Coin {
    image: string;
    count: number;
}

interface Change {
    bills: { [key: string]: Bill };
    coins: { [key: string]: Coin };
}

function CheckoutPage() {
    const [cash, setCash] = useState<number>(0);
    const [change, setChange] = useState<Change>({ bills: {}, coins: {} });
    const [changeBackAmount, setChangeBackAmount] = useState<number>(0);
    const { clearCart, totalAmount, cart } = useCart();
    const [showChangeBreakdown, setShowChangeBreakdown] = useState<boolean>(false);
    const navigate = useNavigate();

    const handleCheckout = async () => {
        if (cash < totalAmount) {
            alert("Insufficient funds!");
            return;
        }
        try {
            const response = await fetch(`${CHECKOUT_VALIDATE_PAYMENT_URL}?totalAmount=${totalAmount}&paidAmount=${cash}`, {
                credentials: 'include'
            });
            if (response.ok) {
                const data: Change = await response.json();
                console.log("Change received:", data);
                await calculateChangeBackAmount();
                setChange(data || { bills: {}, coins: {} });
                setShowChangeBreakdown(true);
                clearCart();
            } else {
                alert("Payment validation failed!");
            }
        } catch (error) {
            console.error("Error during checkout:", error);
            alert("Error processing payment!");
        }
    };

    const calculateChangeBackAmount = async () => {
        try {
            const response = await fetch(`${CHECKOUT_CALCULATE_CHANGE_BACK_AMOUNT_URL}?totalAmount=${totalAmount}&paidAmount=${cash}`);
            if (response.ok) {
                const data = await response.json();
                setChangeBackAmount(data);
                console.log("Change back amount calculated:", data);
            } else {
                alert("Payment validation failed!");
            }
        } catch (error) {
            console.error("Error during checkout:", error);
            alert("Error processing payment!");
        }
    };

    return (
        <div className="checkout-container">
            <header className="checkout-header">
                <h1>Checkout</h1>
            </header>
            {cart.length === 0 ? (
                <p><em>Nothing to pay for. Your cart is empty.</em></p>
            ) : (
                <main className="checkout-content">
                    <div className="input-container">
                        <input
                            type="number"
                            className="styled-input"
                            placeholder="Enter cash amount"
                            onChange={(e) => setCash(Number(e.target.value))}
                        />
                        <span className="currency">{"\u20AC"}</span>
                    </div>
                    <p className="total-amount">Amount To Pay: {"\u20AC"}{totalAmount}</p>
                </main>
            )}
            <div className="change-container">
                {change && showChangeBreakdown && (
                    <>
                        <h2>Change Back: {"\u20AC"}{changeBackAmount}</h2>
                        {changeBackAmount > 0 && (
                            <>
                                <h3>Change Breakdown:</h3>
                                <div className="change-details">
                                    {change?.bills && (
                                        <div className="change-row">
                                            {Object.entries(change.bills).map(([billKey, bill]) => (
                                                <div key={billKey} className="change-item">
                                                    <img
                                                        src={`${bill.image}`}
                                                        alt={`Bill ${billKey}`}
                                                        className="change-image-bill"
                                                        loading="lazy"
                                                    />
                                                    <span>{bill.count} x {"\u20AC"}{billKey}</span>
                                                </div>
                                            ))}
                                        </div>
                                    )}

                                    {change?.coins && (
                                        <div className="change-row">
                                            {Object.entries(change.coins).map(([coinKey, coin]) => (
                                                <div key={coinKey} className="change-item">
                                                    <img
                                                        src={`${coin.image}`}
                                                        alt={`Coin ${coinKey}`}
                                                        className="change-image-coin"
                                                        loading="lazy"
                                                    />
                                                    <span>{coin.count} x {"\u20AC"}{coinKey}</span>
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            </>
                        )}
                    </>
                )}
            </div>

            <footer className="footer">
                {cart.length > 0 && (
                    <>
                        <div className="footer-row">
                            <button className="pay-button" onClick={handleCheckout}>Pay</button>
                        </div>
                    </>
                )}
                <div className="footer-row">
                    <button onClick={() => navigate({ pathname: PRODUCTS_PAGE_URL })}>Back to Store</button>
                    {cart.length > 0 && (
                        <>
                            <button className="reset-button" onClick={clearCart}>Clear Cart</button>
                        </>
                    )}
                </div>
            </footer>
        </div>
    );
}

export default CheckoutPage;
