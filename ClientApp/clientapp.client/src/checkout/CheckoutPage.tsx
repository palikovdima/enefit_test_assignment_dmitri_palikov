import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCart } from "../context/CartContext";
import "./CheckoutPage.css";
import { API_BASE_URL } from '../config/config';

function CheckoutPage() {
    const [cash, setCash] = useState<number>(0);
    const [change, setChange] = useState<any>({});
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
            const response = await fetch(`${ API_BASE_URL }/checkout/validatePayment?totalAmount=${totalAmount}&paidAmount=${cash}`, {
                credentials: 'include'
            });
            if (response.ok) {
                const data = await response.json();
                console.log("Change received:", data);
                await calculateChangeBackAmount();
                setChange(data || {});
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
            const response = await fetch(`${API_BASE_URL }/checkout/calculateChangeBackAmount?totalAmount=${totalAmount}&paidAmount=${cash}`);
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
            ) :  (
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
                )
            }
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
                                                src={`https://localhost:7190/images/Change/Bills/${bill.image}`}
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
                                                src={`https://localhost:7190/images/Change/Coins/${coin.image}`}
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
                    <button onClick={() => navigate('/products')}>Back to Store</button>
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
